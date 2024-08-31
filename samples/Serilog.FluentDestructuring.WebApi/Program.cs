using Serilog;
using Serilog.FluentDestructuring;
using Serilog.FluentDestructuring.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((e, u) =>
    u.Destructure.WithFluentDestructuringPolicy<ApplicationFluentDestructuringPolicy>(o => o.ExcludeTypeTag = true)
        .MinimumLevel.Information()
        .WriteTo.Seq(e.Configuration.GetConnectionString("Seq") ?? throw new ApplicationException()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();