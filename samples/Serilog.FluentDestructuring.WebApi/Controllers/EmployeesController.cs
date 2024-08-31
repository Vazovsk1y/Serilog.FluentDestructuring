using Microsoft.AspNetCore.Mvc;
using Serilog.FluentDestructuring.WebApi.Models;
using Serilog.FluentDestructuring.WebApi.Requests;
using Serilog.FluentDestructuring.WebApi.Responses;

namespace Serilog.FluentDestructuring.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController(ILogger<EmployeesController> logger) : ControllerBase
{
    // Database imitation.
    private static readonly List<Employee> Employees = [];
    
    [HttpPost]
    public IActionResult AddEmployee(EmployeeAddRequest request)
    {
        logger.LogInformation("Employee adding request - {@Request}", request);

        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PassportSeries = request.PassportSeries,
            PassportNumber = request.PassportNumber,
        };

        Employees.Add(employee);
        return Ok();
    }

    [HttpGet("{id}")]
    public IActionResult GetEmployeeById(Guid id)
    {
        var target = Employees.FirstOrDefault(e => e.Id == id);
        if (target is null)
        {
            return BadRequest();
        }

        var response = new EmployeeResponse(
            target.Id,
            target.FirstName,
            target.LastName,
            target.PassportSeries,
            target.PassportNumber,
            target.DateOfBirth,
            target.Post,
            target.Department,
            target.Salary
            );

        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetAllEmployees()
    {
        var response = Employees.Select(e => new EmployeeResponse(
            e.Id,
            e.FirstName,
            e.LastName,
            e.PassportSeries,
            e.PassportNumber,
            e.DateOfBirth,
            e.Post,
            e.Department,
            e.Salary
        ));

        return Ok(response);
    }
    
    
    [HttpPut]
    public IActionResult UpdateEmployee(EmployeeUpdateRequest request)
    {
        logger.LogInformation("Employee updating request - {@Request}", request);

        var target = Employees.FirstOrDefault(e => e.Id == request.EmployeeId);
        if (target is null)
        {
            return BadRequest();
        }

        target.FirstName = request.FirstName;
        target.LastName = request.LastName;
        target.PassportNumber = request.PassportNumber;
        target.PassportSeries = request.PassportSeries;
        target.Post = request.Post;
        target.Salary = request.Salary;
        target.DateOfBirth = request.DateOfBirth;
        target.Department = request.Department;

        return Ok();
    }
}