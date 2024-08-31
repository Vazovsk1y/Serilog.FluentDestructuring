using Serilog.FluentDestructuring.Builders;
using Serilog.FluentDestructuring.Masking;
using Serilog.FluentDestructuring.UnitTests.Models;

namespace Serilog.FluentDestructuring.UnitTests.Infrastructure;

internal sealed class TestFluentDestructuringPolicy : FluentDestructuringPolicy
{
    protected override void Configure(FluentDestructuringBuilder builder)
    {
        builder.Entity<IgnorePropertyModel>(e =>
        {
            e.Property(i => i.Ignore)
                .Ignore();
        });

        builder.Entity<ImmutableAsScalarModel>(e => e.AsScalar());

        builder.Entity<MutableAsScalarModel>(e => e.AsScalar(true));

        builder.Entity<AsScalarPropertyModel>(e =>
        {
            e.Property(o => o.ImmutableAsScalarModel)
                .AsScalar();

            e.Property(o => o.MutableAsScalarModel)
                .AsScalar(true);
        });

        builder.Entity<WithAliasPropertyModel>(e =>
        {
            e.Property(o => o.SimpleProperty)
                .WithAlias("simple_property");
        });

        builder.Entity<MaskPropertyModel>(e =>
        {
            e.Property(o => o.String)
                .Mask();

            e.Property(o => o.StringPreserveLength)
                .Mask(new DefaultMaskingProcessorOptions { PreserveValueLength = true });

            e.Property(o => o.StringWithCustomMaskCharacter)
                .Mask(new DefaultMaskingProcessorOptions { MaskCharacter = '$' });

            e.Property(o => o.StringWithPreserveLengthAndCustomMaskCharacter)
                .Mask(new DefaultMaskingProcessorOptions { PreserveValueLength = true, MaskCharacter = '$' });

            e.Property(o => o.StringWithCustomMaskCharacterAndCustomMaskLength)
                .Mask(new DefaultMaskingProcessorOptions { MaskCharacter = '@', MaskLength = 15 });

            e.Property(o => o.StringCustomMaskingProcessor)
                .Mask(new TestMaskingProcessor());

            e.Property(o => o.StringIEnumerable)
                .Mask();

            e.Property(o => o.NotString)
                .Mask();
        });

        builder.Entity<ConditionalPropertyDestructuringModel>(e =>
        {
            e.Property(o => o.WithAliasIfNotNull)
                .WithAlias("with_alias_if_not_null")
                .ApplyWhenNotNull();

            e.Property(o => o.IgnoreIfNull)
                .Ignore()
                .ApplyWhenNull();

            e.Property(o => o.MaskIfCustomPredicate)
                .Mask()
                .ApplyWhen(i => i.WithAliasIfNotNull == Guid.Empty && i.IgnoreIfNull > 10);
        });

        builder.Entity<AdditionalPropertyDestructuringParametersModel>(e =>
        {
            e.Property(o => o.MaskWithCustomAlias)
                .Mask()
                .WithAlias("mask_with_custom_alias");

            e.Property(o => o.MaskWithCustomAliasAndApplyingPredicate)
                .Mask()
                .WithAlias("mask_with_custom_alias_and_applying_predicate")
                .ApplyWhen(i => i.MaskWithCustomAlias == "value");
        });

        builder.Entity<FluentDestructuringPolicyModel>(e =>
        {
            e.Property(o => o.StringProperty)
                .Mask();
        });
    }
}