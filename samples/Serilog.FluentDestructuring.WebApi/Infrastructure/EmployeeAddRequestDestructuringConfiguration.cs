using Serilog.FluentDestructuring.Builders;
using Serilog.FluentDestructuring.Masking;
using Serilog.FluentDestructuring.WebApi.Requests;

namespace Serilog.FluentDestructuring.WebApi.Infrastructure;

public class EmployeeAddRequestDestructuringConfiguration : IEntityDestructuringConfiguration<EmployeeAddRequest>
{
    public void Configure(EntityDestructuringBuilder<EmployeeAddRequest> builder)
    {
        builder
            .Property(e => e.PassportNumber)
            .Mask(new DefaultMaskingProcessorOptions { MaskLength = 6 })
            .WithAlias("passport_number")
            .ApplyWhen(i => !string.IsNullOrWhiteSpace(i.PassportNumber) && i.PassportNumber.Length == 6);

        builder
            .Property(e => e.PassportSeries)
            .Mask(new DefaultMaskingProcessorOptions { PreserveValueLength = true, MaskCharacter = '#' })
            .WithAlias("passport_series");
    }
}