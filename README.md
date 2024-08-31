# Serilog.FluentDestructuring

This package makes it possible to manipulate how objects are logged to [Serilog](https://serilog.net) using Fluent API.

## Motivation

The [Destructurama.Attributed](https://github.com/destructurama/attributed) package provides convenient ways to configure Serilog complex object logging by using attributes. 
With these, you can easily ignore some properties, apply masking, and so on. But this attribute-based approach does introduce a dependency on Serilog in projects where such a dependency may be undesirable (a similar issue exists with Entity Framework Core and its attribute-based model configuring approach). 
This package emerged out of the need to eliminate this dependency and provide another way for the developers to configure logging using a Fluent API.

## Installation

[Download from NuGet](https://www.nuget.org/packages/Serilog.FluentDestructuring)

##### Package Manager

```powershell
NuGet\Install-Package Serilog.FluentDestructuring -Version *version_number*
```

##### .NET CLI

```cmd
dotnet add package Serilog.FluentDestructuring --version *version_number*
```

## Usage

Define your custom policy and override configure method to specify what destructuring rules to use.

```csharp
public class ApplicationFluentDestructuringPolicy : FluentDestructuringPolicy
{
    protected override void Configure(FluentDestructuringBuilder builder)
    {
        // Add configuration for a specific entity right here.
        builder.Entity<UserRegisterRequest>(e => 
        {
            e.Property(p => p.Email)
                .Mask();
            
            e.Property(p => p.Password)
                .Ignore();
        });
        
        
        // Apply predefined configuration for a specific entity.
        builder.ApplyConfiguration(new UserLoginRequestDestructuringConfiguration());
        
        
        // Apply all entity destructuring configurations found in a specified assembly.
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```

Modify logger configuration.

```csharp
var cfg = new LoggerConfiguration()
    .Destructure.WithFluentDestructuringPolicy<ApplicationFluentDestructuringPolicy>()
    ...
```

### 1. Changing a property name.

Apply by calling `WithAlias` method.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    e.Property(p => p.Email)
        .WithAlias("user_email");
});
```

You can also use custom property name along with a main destructuring rule.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    e.Property(p => p.Email)
        .Mask()
        .WithAlias("user_email");
});
```

### 2. Ignoring a property.

Apply by calling `Ignore` method.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    e.Property(p => p.Password)
        .Ignore();
});
```

### 3. Logging types and properties as scalars.

Apply by calling `AsScalar` method.

```csharp
// Whole entity.
builder.Entity<UserRegisterRequest>(e => e.AsScalar());

builder.Entity<UserRegisterRequest>(e => 
{
    // Individual property.
    e.Property(p => p.Passport)
        .AsScalar();
});
```

### 4. Masking a property.

Apply by calling `Mask` method.

Note that masking works for properties of type `string`, `IEnumerable<string>` or derived from it, for example, `string[]` or `List<string>`.

#### 4.1. Default behaviour.

- **MaskCharacter:** The character used for masking. The default is an asterisk `*`.
- **MaskLength:** The length of the mask to be applied. This is the number of `MaskCharacter` that will be used to obfuscate the value. The default is `10`.
- **PreserveValueLength:** Value indicating whether the length of the original value should be preserved when applying the mask. If `true`, the masked value will have the same length as the original value, and the `MaskLength` property will be ignored. The default is `false`.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    // Default masking processor with default options.
    e.Property(p => p.Email)
        .Mask();
    
    // Customize default masking processor behaviour by specify options.
    e.Property(e => e.Password)
       .Mask(new DefaultMaskingProcessorOptions { PreserveValueLength = true, MaskCharacter = '#' })
});
```

#### 4.2. Custom behaviour.

You can use custom masking processor by implementing the `IMaskingProcessor` interface and passing an instance to one of the overloads of `Mask` method.

```csharp
public class PasswordMaskingProcessor : IMaskingProcessor
{
    public bool TryMask(string value, out string? maskedValue)
    {
        // Your implementation.
    }
}
```

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    e.Property(e => e.Password)
       .Mask(new PasswordMaskingProcessor());
});
```

### 5. Conditional property destructuring applying.

You can define the condition under which the destructuring rule will be applied.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    // One of predefined conditions.
    e.Property(p => p.Email)
        .Ignore()
        .ApplyWhenNull();
    
    e.Property(e => e.Passport)
       .AsScalar()
       .WithAlias("user_passport")
       .ApplyWhenNotNull();
    
    
    // Define your custom condition.
    e.Property(p => p.Password)
        .Mask()
        .ApplyWhen(e => !string.IsNullOrWhiteSpace(e.Email) && e.Email.EndsWith("@gmail.com"));
});
```

### 6. Inner entities.

Only single-level properties are supported.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{
    // Will throw an exception.  
    e.Property(e => e.Passport.Series)
       .Mask();
});
```

Configure inner entities by calling `InnerEntity` method.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{  
    e.InnerEntity(o => o.Passport, x =>
    {
        x.Property(a => a.Series)
            .Mask();

        x.Property(a => a.Number)
            .Ignore()
            .ApplyWhenNull();
    })
    .WithAlias("user_passport")
    .ApplyWhen(e => !string.IsNullOrWhiteSpace(e.Email));
});
```
Or apply predefined configuration.

```csharp
builder.Entity<UserRegisterRequest>(e => 
{  
    e.InnerEntity(o => o.Passport, new PassportDestructuringConfiguration())
        .WithAlias("user_passport")
        .ApplyWhen(e => !string.IsNullOrWhiteSpace(e.Email));
});
```

### 7. Global options.

- **IgnoreNullProperties** - Indicating whether properties with null values should be ignored during destructuring. The default is `false`.
- **ExcludeTypeTag** - Indicating whether the `$type` tag should be excluded from the destructured output. The default is `false`.

```csharp
var cfg = new LoggerConfiguration()
    .Destructure.WithFluentDestructuringPolicy<ApplicationFluentDestructuringPolicy>(e => {
        e.IgnoreNullProperties = true;
        e.ExcludeTypeTag = true;
    })
    ...
```