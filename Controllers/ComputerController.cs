using LabBack.Contracts;
using LabBack.Contracts.Responses;
using LabBack.Models;
using LabBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabBack.Controllers;

/// <summary>
/// Exposes API endpoints for managing PCs and servers in the computer lab.
/// </summary>
[ApiController]
[Route("api/computers")]
public class ComputerController : ControllerBase
{
    private readonly IComputerLabService _computerLabService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ComputerController"/> class.
    /// </summary>
    /// <param name="computerLabService">Computer lab service used for data access.</param>
    public ComputerController(IComputerLabService computerLabService)
    {
        _computerLabService = computerLabService;
    }

    /// <summary>
    /// Returns all PCs stored in the lab.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The list of PCs.</returns>
    [HttpGet("pcs")]
    [ProducesResponseType(typeof(IEnumerable<PcSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PcSummaryResponse>>> GetAllPcs(CancellationToken cancellationToken)
    {
        var pcs = await _computerLabService.GetAllPcsAsync(cancellationToken);
        var response = pcs.Select(pc => new PcSummaryResponse
        {
            Id = pc.Id,
            ProcessorFrequency = pc.ProcessorFrequency,
            RamAmount = pc.RamAmount,
            UserShell = pc.UserShell,
            Os = pc.Os
        });

        return Ok(response);
    }

    /// <summary>
    /// Returns all servers stored in the lab.
    /// </summary>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The list of servers.</returns>
    [HttpGet("servers")]
    [ProducesResponseType(typeof(IEnumerable<ServerSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServerSummaryResponse>>> GetAllServers(CancellationToken cancellationToken)
    {
        var servers = await _computerLabService.GetAllServersAsync(cancellationToken);
        var response = servers.Select(server => new ServerSummaryResponse
        {
            Id = server.Id,
            ProcessorFrequency = server.ProcessorFrequency,
            RamAmount = server.RamAmount,
            MaxConnections = server.MaxConnections,
            CurrentConnections = server.CurrentConnections
        });

        return Ok(response);
    }

    /// <summary>
    /// Creates a new PC.
    /// </summary>
    /// <param name="request">Request payload with PC settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Identifier and creation message.</returns>
    [HttpPost("pcs")]
    [ProducesResponseType(typeof(CreatedResourceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResourceResponse>> CreatePc([FromBody] CreatePcRequest request, CancellationToken cancellationToken)
    {
        var userShell = string.IsNullOrWhiteSpace(request.UserShell) ? "XFCE" : request.UserShell;
        var os = string.IsNullOrWhiteSpace(request.Os) ? "Linux" : request.Os;

        var pc = new PC(request.ProcessorFrequency, request.RamAmount, userShell, os);
        var id = await _computerLabService.AddPcAsync(pc, cancellationToken);

        return CreatedAtAction(nameof(GetPcInfo), new { id }, new CreatedResourceResponse
        {
            Id = id,
            Message = "PC created"
        });
    }

    /// <summary>
    /// Creates a new server.
    /// </summary>
    /// <param name="request">Request payload with server settings.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>Identifier and creation message.</returns>
    [HttpPost("servers")]
    [ProducesResponseType(typeof(CreatedResourceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResourceResponse>> CreateServer([FromBody] CreateServerRequest request, CancellationToken cancellationToken)
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

        return CreatedAtAction(nameof(GetServerInfo), new { id }, new CreatedResourceResponse
        {
            Id = id,
            Message = "Server created"
        });
    }

    /// <summary>
    /// Returns the human-readable information string for a PC.
    /// </summary>
    /// <param name="id">PC identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the PC.</returns>
    [HttpGet("pcs/{id:int}/info")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> GetPcInfo([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"PC with id {id} was not found.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = pc.DisplayInfo()
        });
    }

    /// <summary>
    /// Returns the human-readable information string for a server.
    /// </summary>
    /// <param name="id">Server identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the server.</returns>
    [HttpGet("servers/{id:int}/info")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> GetServerInfo([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = server.DisplayInfo()
        });
    }

    /// <summary>
    /// Simulates a task on the selected PC.
    /// </summary>
    /// <param name="id">PC identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the executed task.</returns>
    [HttpPost("pcs/{id:int}/execute")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> ExecutePcTask([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"PC with id {id} was not found.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = pc.ExecuteTask()
        });
    }

    /// <summary>
    /// Simulates a task on the selected server.
    /// </summary>
    /// <param name="id">Server identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the executed task.</returns>
    [HttpPost("servers/{id:int}/execute")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> ExecuteServerTask([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = server.ExecuteTask()
        });
    }

    /// <summary>
    /// Opens an application on the selected PC.
    /// </summary>
    /// <param name="id">PC identifier.</param>
    /// <param name="request">Request payload with the application name.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the action.</returns>
    [HttpPost("pcs/{id:int}/open-application")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> OpenPcApplication([FromRoute] int id, [FromBody] OpenApplicationRequest request, CancellationToken cancellationToken)
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

        return Ok(new OperationMessageResponse
        {
            Message = pc.OpenApplication(request.AppName)
        });
    }

    /// <summary>
    /// Opens a website on the selected PC.
    /// </summary>
    /// <param name="id">PC identifier.</param>
    /// <param name="request">Request payload with the website URL.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the action.</returns>
    [HttpPost("pcs/{id:int}/open-website")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> OpenPcWebsite([FromRoute] int id, [FromBody] OpenWebsiteRequest request, CancellationToken cancellationToken)
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

        return Ok(new OperationMessageResponse
        {
            Message = pc.OpenWebsite(request.Url)
        });
    }

    /// <summary>
    /// Accepts a new connection on the selected server.
    /// </summary>
    /// <param name="id">Server identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The updated connection counters.</returns>
    [HttpPost("servers/{id:int}/accept-connection")]
    [ProducesResponseType(typeof(ConnectionStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConnectionStatusResponse>> AcceptServerConnection([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        var accepted = server.AcceptConnection(out var message);
        if (!accepted)
        {
            return Conflict(new OperationMessageResponse
            {
                Message = message
            });
        }

        await _computerLabService.SaveChangesAsync(cancellationToken);
        return Ok(new ConnectionStatusResponse
        {
            Message = message,
            CurrentConnections = server.CurrentConnections,
            MaxConnections = server.MaxConnections
        });
    }

    /// <summary>
    /// Simulates processing a network request on the selected server.
    /// </summary>
    /// <param name="id">Server identifier.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A message describing the server activity.</returns>
    [HttpPost("servers/{id:int}/process-request")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> ProcessServerRequest([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Server with id {id} was not found.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = server.ProcessRequest()
        });
    }
}
