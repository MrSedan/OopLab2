using LabBack.Contracts;
using LabBack.Contracts.Responses;
using LabBack.Models;
using LabBack.Services;
using Microsoft.AspNetCore.Mvc;

namespace LabBack.Controllers;

/// <summary>
/// Описывает API-эндпоинты для управления ПК и серверами в компьютерном классе.
/// </summary>
[ApiController]
[Route("api/computers")]
public class ComputerController : ControllerBase
{
    private readonly IComputerLabService _computerLabService;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="ComputerController"/>.
    /// </summary>
    /// <param name="computerLabService">Сервис компьютерного класса для работы с данными.</param>
    public ComputerController(IComputerLabService computerLabService)
    {
        _computerLabService = computerLabService;
    }

    /// <summary>
    /// Возвращает все ПК, сохраненные в системе.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список ПК.</returns>
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
    /// Возвращает все серверы, сохраненные в системе.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список серверов.</returns>
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
    /// Создает новый ПК.
    /// </summary>
    /// <param name="request">Параметры ПК.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор и сообщение о создании.</returns>
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
            Message = "ПК создан"
        });
    }

    /// <summary>
    /// Обновляет существующий ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="request">Новые параметры ПК.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновленное краткое описание ПК.</returns>
    [HttpPut("pcs/{id:int}")]
    [ProducesResponseType(typeof(PcSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PcSummaryResponse>> UpdatePc([FromRoute] int id, [FromBody] CreatePcRequest request, CancellationToken cancellationToken)
    {
        var userShell = string.IsNullOrWhiteSpace(request.UserShell) ? "XFCE" : request.UserShell;
        var os = string.IsNullOrWhiteSpace(request.Os) ? "Linux" : request.Os;
        var updatedPc = new PC(request.ProcessorFrequency, request.RamAmount, userShell, os);

        var pc = await _computerLabService.UpdatePcAsync(id, updatedPc, cancellationToken);
        if (pc is null)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        return Ok(new PcSummaryResponse
        {
            Id = pc.Id,
            ProcessorFrequency = pc.ProcessorFrequency,
            RamAmount = pc.RamAmount,
            UserShell = pc.UserShell,
            Os = pc.Os
        });
    }

    /// <summary>
    /// Увеличивает объем оперативной памяти существующего ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="request">Количество памяти для добавления.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновленное краткое описание ПК.</returns>
    [HttpPost("pcs/{id:int}/increase-ram")]
    [ProducesResponseType(typeof(PcSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PcSummaryResponse>> IncreasePcRam(
        [FromRoute] int id,
        [FromBody] IncreasePcRamRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Amount < 0)
        {
            return BadRequest("Количество памяти не может быть отрицательным.");
        }

        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        pc.IncreaseRamAmount(request.Amount);
        await _computerLabService.SaveChangesAsync(cancellationToken);

        return Ok(new PcSummaryResponse
        {
            Id = pc.Id,
            ProcessorFrequency = pc.ProcessorFrequency,
            RamAmount = pc.RamAmount,
            UserShell = pc.UserShell,
            Os = pc.Os
        });
    }

    /// <summary>
    /// Удаляет существующий ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    [HttpDelete("pcs/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePc([FromRoute] int id, CancellationToken cancellationToken)
    {
        var deleted = await _computerLabService.DeletePcAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        return NoContent();
    }

    /// <summary>
    /// Создает новый сервер.
    /// </summary>
    /// <param name="request">Параметры сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Идентификатор и сообщение о создании.</returns>
    [HttpPost("servers")]
    [ProducesResponseType(typeof(CreatedResourceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreatedResourceResponse>> CreateServer([FromBody] CreateServerRequest request, CancellationToken cancellationToken)
    {
        if (request.MaxConnections < 0 || request.CurrentConnections < 0)
        {
            return BadRequest("Число подключений не может быть отрицательным.");
        }

        if (request.CurrentConnections > request.MaxConnections)
        {
            return BadRequest("Текущее число подключений не может превышать максимальное.");
        }

        var server = new Server(request.ProcessorFrequency, request.RamAmount, request.MaxConnections, request.CurrentConnections);
        var id = await _computerLabService.AddServerAsync(server, cancellationToken);

        return CreatedAtAction(nameof(GetServerInfo), new { id }, new CreatedResourceResponse
        {
            Id = id,
            Message = "Сервер создан"
        });
    }

    /// <summary>
    /// Обновляет существующий сервер.
    /// </summary>
    /// <param name="id">Идентификатор сервера.</param>
    /// <param name="request">Новые параметры сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновленное краткое описание сервера.</returns>
    [HttpPut("servers/{id:int}")]
    [ProducesResponseType(typeof(ServerSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServerSummaryResponse>> UpdateServer([FromRoute] int id, [FromBody] CreateServerRequest request, CancellationToken cancellationToken)
    {
        if (request.MaxConnections < 0 || request.CurrentConnections < 0)
        {
            return BadRequest("Число подключений не может быть отрицательным.");
        }

        if (request.CurrentConnections > request.MaxConnections)
        {
            return BadRequest("Текущее число подключений не может превышать максимальное.");
        }

        var updatedServer = new Server(request.ProcessorFrequency, request.RamAmount, request.MaxConnections, request.CurrentConnections);
        var server = await _computerLabService.UpdateServerAsync(id, updatedServer, cancellationToken);
        if (server is null)
        {
            return NotFound($"Сервер с идентификатором {id} не найден.");
        }

        return Ok(new ServerSummaryResponse
        {
            Id = server.Id,
            ProcessorFrequency = server.ProcessorFrequency,
            RamAmount = server.RamAmount,
            MaxConnections = server.MaxConnections,
            CurrentConnections = server.CurrentConnections
        });
    }

    /// <summary>
    /// Удаляет существующий сервер.
    /// </summary>
    /// <param name="id">Идентификатор сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Пустой ответ при успешном удалении.</returns>
    [HttpDelete("servers/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteServer([FromRoute] int id, CancellationToken cancellationToken)
    {
        var deleted = await _computerLabService.DeleteServerAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound($"Сервер с идентификатором {id} не найден.");
        }

        return NoContent();
    }

    /// <summary>
    /// Возвращает человекочитаемую информацию о ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение с описанием ПК.</returns>
    [HttpGet("pcs/{id:int}/info")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> GetPcInfo([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = pc.DisplayInfo()
        });
    }

    /// <summary>
    /// Возвращает человекочитаемую информацию о сервере.
    /// </summary>
    /// <param name="id">Идентификатор сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение с описанием сервера.</returns>
    [HttpGet("servers/{id:int}/info")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> GetServerInfo([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Сервер с идентификатором {id} не найден.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = server.DisplayInfo()
        });
    }

    /// <summary>
    /// Выполняет имитацию задачи на выбранном ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение о выполненной задаче.</returns>
    [HttpPost("pcs/{id:int}/execute")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> ExecutePcTask([FromRoute] int id, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = pc.ExecuteTask()
        });
    }

    /// <summary>
    /// Выполняет имитацию задачи на выбранном сервере.
    /// </summary>
    /// <param name="id">Идентификатор сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение о выполненной задаче.</returns>
    [HttpPost("servers/{id:int}/execute")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> ExecuteServerTask([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Сервер с идентификатором {id} не найден.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = server.ExecuteTask()
        });
    }

    /// <summary>
    /// Открывает приложение на выбранном ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="request">Название приложения.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение о выполненном действии.</returns>
    [HttpPost("pcs/{id:int}/open-application")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> OpenPcApplication([FromRoute] int id, [FromBody] OpenApplicationRequest request, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        if (string.IsNullOrWhiteSpace(request.AppName))
        {
            return BadRequest("Имя приложения обязательно.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = pc.OpenApplication(request.AppName)
        });
    }

    /// <summary>
    /// Открывает веб-сайт на выбранном ПК.
    /// </summary>
    /// <param name="id">Идентификатор ПК.</param>
    /// <param name="request">URL веб-сайта.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение о выполненном действии.</returns>
    [HttpPost("pcs/{id:int}/open-website")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> OpenPcWebsite([FromRoute] int id, [FromBody] OpenWebsiteRequest request, CancellationToken cancellationToken)
    {
        var pc = await _computerLabService.GetPcAsync(id, cancellationToken);
        if (pc is null)
        {
            return NotFound($"ПК с идентификатором {id} не найден.");
        }

        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL обязателен.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = pc.OpenWebsite(request.Url)
        });
    }

    /// <summary>
    /// Принимает новое подключение на выбранном сервере.
    /// </summary>
    /// <param name="id">Идентификатор сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновленные счетчики подключений.</returns>
    [HttpPost("servers/{id:int}/accept-connection")]
    [ProducesResponseType(typeof(ConnectionStatusResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConnectionStatusResponse>> AcceptServerConnection([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Сервер с идентификатором {id} не найден.");
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
    /// Выполняет имитацию обработки сетевого запроса на выбранном сервере.
    /// </summary>
    /// <param name="id">Идентификатор сервера.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Сообщение о работе сервера.</returns>
    [HttpPost("servers/{id:int}/process-request")]
    [ProducesResponseType(typeof(OperationMessageResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OperationMessageResponse>> ProcessServerRequest([FromRoute] int id, CancellationToken cancellationToken)
    {
        var server = await _computerLabService.GetServerAsync(id, cancellationToken);
        if (server is null)
        {
            return NotFound($"Сервер с идентификатором {id} не найден.");
        }

        return Ok(new OperationMessageResponse
        {
            Message = server.ProcessRequest()
        });
    }
}
