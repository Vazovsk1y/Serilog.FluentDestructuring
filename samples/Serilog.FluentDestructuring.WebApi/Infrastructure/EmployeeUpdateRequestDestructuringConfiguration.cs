using Serilog.FluentDestructuring.Builders;
using Serilog.FluentDestructuring.Masking;
using Serilog.FluentDestructuring.WebApi.Requests;

namespace Serilog.FluentDestructuring.WebApi.Infrastructure;

public class EmployeeUpdateRequestDestructuringConfiguration : IEntityDestructuringConfiguration<EmployeeUpdateRequest>
{
    public void Configure(EntityDestructuringBuilder<EmployeeUpdateRequest> builder)
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

        builder
            .Property(e => e.Salary)
            .Ignore()
            .ApplyWhenNotNull();

        builder
            .Property(e => e.DateOfBirth)
            .Ignore();

        builder
            .Property(e => e.Post)
            .WithAlias("employee_post");
    }
}