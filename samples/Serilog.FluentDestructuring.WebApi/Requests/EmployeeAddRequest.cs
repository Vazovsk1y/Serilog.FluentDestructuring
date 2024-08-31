namespace Serilog.FluentDestructuring.WebApi.Requests;

public record EmployeeAddRequest(
    string FirstName,
    string LastName,
    string PassportSeries,
    string PassportNumber
    );