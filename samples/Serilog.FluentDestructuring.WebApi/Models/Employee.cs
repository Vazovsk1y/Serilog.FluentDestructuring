namespace Serilog.FluentDestructuring.WebApi.Models;

public class Employee
{
    public required Guid Id { get; init; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string PassportSeries { get; set; }
    
    public required string PassportNumber { get; set; }
    
    public DateOnly? DateOfBirth { get; set; }
    
    public string? Post { get; set; }
    
    public string? Department { get; set; }
    
    public decimal? Salary { get; set; }
}
