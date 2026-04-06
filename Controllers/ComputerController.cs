using LabBack.Contracts;
using LabBack.Models;
using LabBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabBack.Controllers;

[ApiController]
[Route("api/computers")]
public class ComputerController : ControllerBase
{
    private readonly IComputerLabService _computerLabService;

    public ComputerController(IComputerLabService computerLabService)
    {
        _computerLabService = computerLabService;
    }

    [HttpGet("pcs")]
    public async Task<IActionResult> GetAllPcs(CancellationToken cancellationToken)
    {
        var pcs = await _computerLabService.GetAllPcsAsync(cancellationToken);
        var response = pcs.Select(pc => new
            {
                pc.Id,
                pc.ProcessorFrequency,
                pc.RamAmount,
                pc.UserShell,
                pc.Os
            });

        return Ok(response);
    }

    [HttpGet("servers")]
    public async Task<IActionResult> GetAllServers(CancellationToken cancellationToken)
    {
        var servers = await _computerLabService.GetAllServersAsync(cancellationToken);
        var response = servers.Select(server => new
            {
                server.Id,
                server.ProcessorFrequency,
                server.RamAmount,
                server.MaxConnections,
                server.CurrentConnections
            });

        return Ok(response);
    }

    [HttpPost("pcs")]
    public async Task<IActionResult> CreatePc([FromBody] CreatePcRequest request, CancellationToken cancellationToken)
    {
        var userShell = string.IsNullOrWhiteSpace(request.UserShell) ? "XFCE" : request.UserShell;
        var os = string.IsNullOrWhiteSpace(request.Os) ? "Linux" : request.Os;

        var pc = new PC(request.ProcessorFrequency, request.RamAmount, userShell, os);
        var id = await _computerLabService.AddPcAsync(pc, cancellationToken);

        return CreatedAtAction(nameof(GetPcInfo), new { id }, new { Id = id, Message = "PC created" });
    }

    [HttpPost("servers")]
    public async Task<IActionResult> CreateServer([FromBody] CreateServerRequest request, CancellationToken cancellationToken)
    {
        if (request.MaxConnections < 0 || request.CurrentConnections < 0)
        {
            return BadRequest("Connections cannot be negative.");
        }

        if (request.CurrentConnections > request.MaxConnections)
        {
            return BadRequest("CurrentConnections cannot exceed MaxConnections.");
        }

        var server = new Server(request.ProcessorFrequency, request.RamAmount, request.MaxConnections, request.CurrentConnections);
        var id = await _computerLabService.AddServerAsync(server, cancellationToken);

        return CreatedAtAction(nameof(GetServerInfo), new { id }, new { Id = id, Message = "Server created" });
    }

    [HttpGet("pcs/{id:int}/info")]
    public async Task<IActionResult> GetPcInfo([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"PC with id {id} was not found.");
        }

        return Ok(new { Message = pc.DisplayInfo() });
    }

    [HttpGet("servers/{id:int}/info")]
    public async Task<IActionResult> GetServerInfo([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        return Ok(new { Message = server.DisplayInfo() });
    }

    [HttpPost("pcs/{id:int}/execute")]
    public async Task<IActionResult> ExecutePcTask([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"PC with id {id} was not found.");
        }

        return Ok(new { Message = pc.ExecuteTask() });
    }

    [HttpPost("servers/{id:int}/execute")]
    public async Task<IActionResult> ExecuteServerTask([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        return Ok(new { Message = server.ExecuteTask() });
    }

    [HttpPost("pcs/{id:int}/open-application")]
    public async Task<IActionResult> OpenPcApplication([FromRoute] int id, [FromBody] OpenApplicationRequest request, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"PC with id {id} was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.AppName))
        {
            return BadRequest("AppName is required.");
        }

        return Ok(new { Message = pc.OpenApplication(request.AppName) });
    }

    [HttpPost("pcs/{id:int}/open-website")]
    public async Task<IActionResult> OpenPcWebsite([FromRoute] int id, [FromBody] OpenWebsiteRequest request, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"PC with id {id} was not found.");
        }

        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("Url is required.");
        }

        return Ok(new { Message = pc.OpenWebsite(request.Url) });
    }

    [HttpPost("servers/{id:int}/accept-connection")]
    public async Task<IActionResult> AcceptServerConnection([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        var accepted = server.AcceptConnection(out var message);
        if (!accepted)
        {
            return Conflict(new { Message = message });
        }

        await _computerLabService.SaveChangesAsync(cancellationToken);
        return Ok(new { Message = message, server.CurrentConnections, server.MaxConnections });
    }

    [HttpPost("servers/{id:int}/process-request")]
    public async Task<IActionResult> ProcessServerRequest([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        return Ok(new { Message = server.ProcessRequest() });
    }
}
