using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Commands.CreateEmployee;
using EmployeeManagement.Application.Employees.Commands.DeleteEmployee;
using EmployeeManagement.Application.Employees.Commands.PatchEmployee;
using EmployeeManagement.Application.Employees.Commands.UpdateEmployee;
using EmployeeManagement.Application.Employees.Queries.GetEmployeeById;
using EmployeeManagement.Application.Employees.Queries.GetEmployees;
using EmployeeManagement.Infrastructure.Jobs;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/employees")]
[EnableRateLimiting("FixedWindow")]
public class EmployeesController(
    IMediator mediator,
    IEmployeeReportService reportService,
    IBackgroundJobClient backgroundJobClient) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IEmployeeReportService _reportService = reportService;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;


    [Authorize]
    [HttpPost("report/daily")]
    public async Task<IActionResult> GenerateDailyReport()
    {
        await _reportService.GenerateDailyReport();
        return Ok("Daily report generated successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "CanDeleteEmployee")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] PatchEmployeeCommand command)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        await _mediator.Send(command);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "CanDeleteEmployee")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteEmployeeCommand(id));
        return NoContent();
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetEmployeesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
        return Ok(employee);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet("admins")]
    public async Task<IActionResult> GetAdmins()
    {
        var admins = await _mediator.Send(new GetAdminsQuery());
        return Ok(admins);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB max
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only CSV files are accepted.");

        // Save to a temp file — job reads from disk so the request stream stays open
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"employees_{Guid.NewGuid()}.csv");
        await using (var stream = new FileStream(tempPath, FileMode.Create))
            await file.CopyToAsync(stream);

        // Enqueue background job — ProcessEmployeeCsvJob.ExecuteAsync(tempPath)
        var jobId = _backgroundJobClient.Enqueue<ProcessEmployeeCsvJob>(
            job => job.ExecuteAsync(tempPath));

        return Accepted(new { jobId, message = "CSV upload accepted. Processing in background." });
    }
}