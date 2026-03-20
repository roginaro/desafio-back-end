using Desafio.Application.Models;
using Desafio.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpPost]
    public async Task<ActionResult<StatusResponse>> ProcessarStatus([FromBody] StatusRequest request)
    {

        var response = await _statusService.ProcessarStatusAsync(request);
        return Ok(response);

    }
}