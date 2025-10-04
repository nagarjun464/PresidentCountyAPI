using Microsoft.AspNetCore.Mvc;
using PresidentCountyAPI.Models;
using PresidentCountyAPI.Services;

namespace PresidentCountyAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PresidentCountyController : ControllerBase
{
    private readonly PresidentCountyService _service;
    private readonly ReportService _reportService;


    public PresidentCountyController(IConfiguration config)
    {
        var projectId = config["GoogleCloud:ProjectId"];
        var credentialsPath = config["GoogleCloud:CredentialsPath"];
        _service = new PresidentCountyService(projectId, credentialsPath);
        _reportService = new ReportService();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(string? search = null, int page = 1, int pageSize = 50)
    {
        Console.WriteLine("search is {search}");
        var results = await _service.GetPagedAsync(page, pageSize, search);
        Console.WriteLine("results is {results}");
        return Ok(results);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _service.GetAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PresidentCountyCandidate candidate)
    {
        var id = await _service.CreateAsync(candidate);
        return CreatedAtAction(nameof(Get), new { id }, candidate);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] PresidentCountyCandidate candidate)
    {
        await _service.UpdateAsync(id, candidate);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel(string? search = null)
    {
        var list = await _service.GetAllFiltered(search);
        var bytes = _reportService.ExportPresidentCountyToExcel(list);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"PresidentCountyReport_{search}.xlsx");
    }

    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf(string? search = null)
    {
        try
        {
            var list = await _service.GetAllFiltered(search);
            if (list == null || !list.Any())
                return BadRequest("No data available to export.");

            var bytes = _reportService.ExportPresidentCountyToPdf(list);
            return File(bytes, "application/pdf", "PresidentCountyReport.pdf");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PDF Export Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return StatusCode(500, $"PDF export failed: {ex.Message}");
        }

    }
}