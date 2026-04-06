using LabBack.Controllers;
using LabBack.Contracts;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace LabBack.Swagger;

/// <summary>
/// Добавляет теги ресурсов и подобранные примеры запросов и ответов для API компьютеров.
/// </summary>
public sealed class ComputerApiOperationFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Tags = new HashSet<OpenApiTagReference>
        {
            new(GetTagName(context.ApiDescription.RelativePath), null, null)
        };

        switch (context.MethodInfo.Name)
        {
            case nameof(ComputerController.CreatePc):
            case nameof(ComputerController.UpdatePc):
                SetRequestExample(operation, BuildCreatePcRequestExample());
                SetResponseExample(operation, "201", BuildCreatedResourceExample("ПК создан", 1));
                SetResponseExample(operation, "200", BuildPcSummaryExample());
                break;
            case nameof(ComputerController.CreateServer):
            case nameof(ComputerController.UpdateServer):
                SetRequestExample(operation, BuildCreateServerRequestExample());
                SetResponseExample(operation, "201", BuildCreatedResourceExample("Сервер создан", 2));
                SetResponseExample(operation, "200", BuildServerSummaryExample());
                break;
            case nameof(ComputerController.GetAllPcs):
                SetResponseExample(operation, "200", BuildPcListExample());
                break;
            case nameof(ComputerController.GetAllServers):
                SetResponseExample(operation, "200", BuildServerListExample());
                break;
            case nameof(ComputerController.GetPcInfo):
            case nameof(ComputerController.ExecutePcTask):
                SetResponseExample(operation, "200", BuildMessageExample("ПК обрабатывает пользовательские приложения и офисные задачи"));
                break;
            case nameof(ComputerController.GetServerInfo):
                SetResponseExample(operation, "200", BuildMessageExample("Сервер: ОЗУ 16384 МБ, частота процессора 2800 МГц, активных подключений 12 из максимально возможных 100"));
                break;
            case nameof(ComputerController.ExecuteServerTask):
                SetResponseExample(operation, "200", BuildMessageExample("Сервер обрабатывает сетевые запросы клиентов"));
                break;
            case nameof(ComputerController.OpenPcApplication):
                SetRequestExample(operation, BuildOpenApplicationRequestExample());
                SetResponseExample(operation, "200", BuildMessageExample("ПК открывает приложение: LibreOffice"));
                break;
            case nameof(ComputerController.OpenPcWebsite):
                SetRequestExample(operation, BuildOpenWebsiteRequestExample());
                SetResponseExample(operation, "200", BuildMessageExample("ПК открывает сайт: https://example.com"));
                break;
            case nameof(ComputerController.AcceptServerConnection):
                SetResponseExample(operation, "200", BuildConnectionStatusExample());
                SetResponseExample(operation, "409", BuildMessageExample("Сервер перегружен. Невозможно принять новое подключение."));
                break;
            case nameof(ComputerController.ProcessServerRequest):
                SetResponseExample(operation, "200", BuildMessageExample("Сервер обрабатывает сетевой запрос клиента"));
                break;
        }
    }

    private static string GetTagName(string? relativePath)
    {
        return relativePath?.Contains("/servers", StringComparison.OrdinalIgnoreCase) == true ? "Серверы" : "ПК";
    }

    private static void SetRequestExample(OpenApiOperation operation, JsonNode example)
    {
        if (operation.RequestBody?.Content?.TryGetValue("application/json", out var mediaType) == true)
        {
            mediaType.Example = example;
        }
    }

    private static void SetResponseExample(OpenApiOperation operation, string statusCode, JsonNode example)
    {
        var responses = operation.Responses;
        if (responses is not null && responses.TryGetValue(statusCode, out var response))
        {
            var content = response.Content;
            if (content is not null && content.TryGetValue("application/json", out var mediaType))
            {
                mediaType.Example = example;
            }
        }
    }

    private static JsonObject BuildCreatePcRequestExample()
    {
        return new JsonObject
        {
            ["processorFrequency"] = 3200,
            ["ramAmount"] = 8192,
            ["userShell"] = "GNOME",
            ["os"] = "Linux"
        };
    }

    private static JsonObject BuildCreateServerRequestExample()
    {
        return new JsonObject
        {
            ["processorFrequency"] = 2800,
            ["ramAmount"] = 16384,
            ["maxConnections"] = 100,
            ["currentConnections"] = 12
        };
    }

    private static JsonObject BuildOpenApplicationRequestExample()
    {
        return new JsonObject
        {
            ["appName"] = "LibreOffice"
        };
    }

    private static JsonObject BuildOpenWebsiteRequestExample()
    {
        return new JsonObject
        {
            ["url"] = "https://example.com"
        };
    }

    private static JsonArray BuildPcListExample()
    {
        return new JsonArray
        {
            new JsonObject
            {
                ["id"] = 1,
                ["processorFrequency"] = 3200,
                ["ramAmount"] = 8192,
                ["userShell"] = "GNOME",
                ["os"] = "Linux"
            }
        };
    }

    private static JsonObject BuildPcSummaryExample()
    {
        return new JsonObject
        {
            ["id"] = 1,
            ["processorFrequency"] = 3200,
            ["ramAmount"] = 8192,
            ["userShell"] = "GNOME",
            ["os"] = "Linux"
        };
    }

    private static JsonArray BuildServerListExample()
    {
        return new JsonArray
        {
            new JsonObject
            {
                ["id"] = 2,
                ["processorFrequency"] = 2800,
                ["ramAmount"] = 16384,
                ["maxConnections"] = 100,
                ["currentConnections"] = 12
            }
        };
    }

    private static JsonObject BuildServerSummaryExample()
    {
        return new JsonObject
        {
            ["id"] = 2,
            ["processorFrequency"] = 2800,
            ["ramAmount"] = 16384,
            ["maxConnections"] = 100,
            ["currentConnections"] = 12
        };
    }

    private static JsonObject BuildCreatedResourceExample(string message, int id)
    {
        return new JsonObject
        {
            ["id"] = id,
            ["message"] = message
        };
    }

    private static JsonObject BuildMessageExample(string message)
    {
        return new JsonObject
        {
            ["message"] = message
        };
    }

    private static JsonObject BuildConnectionStatusExample()
    {
        return new JsonObject
        {
            ["message"] = "Сервер принял новое подключение. Текущее количество подключений: 13",
            ["currentConnections"] = 13,
            ["maxConnections"] = 100
        };
    }
}
