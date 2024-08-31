namespace Serilog.FluentDestructuring.WebApi.Requests;

public record EmployeeUpdateRequest(
    Guid EmployeeId,
    string FirstName,
    string LastName,
    string PassportSeries,
    string PassportNumber,
    DateOnly? DateOfBirth,
    string? Post,
    string? Department,
    decimal? Salary
    );