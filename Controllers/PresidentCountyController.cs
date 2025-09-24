using Microsoft.AspNetCore.Mvc;
using PresidentCountyAPI.Models;
using PresidentCountyAPI.Services;

namespace PresidentCountyAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PresidentCountyController : ControllerBase
{
    private readonly PresidentCountyService _service;

    public PresidentCountyController(IConfiguration config)
    {
        var projectId = config["GoogleCloud:ProjectId"];
        var credentialsPath = config["GoogleCloud:CredentialsPath"];
        _service = new PresidentCountyService(projectId, credentialsPath);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 50)
    {
        var results = await _service.GetPagedAsync(page, pageSize);
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
}
