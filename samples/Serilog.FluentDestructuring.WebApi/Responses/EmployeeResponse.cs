namespace Serilog.FluentDestructuring.WebApi.Responses;

public record EmployeeResponse(
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