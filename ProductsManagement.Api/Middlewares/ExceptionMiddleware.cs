using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ProductsManagement.Domain.Exceptions;

namespace ProductsManagement.Api.Middlewares;

/// <summary>
/// Middleware global de manejo de excepciones.
/// </summary>
/// <remarks>
/// Centraliza el manejo de errores de la aplicación y los traduce a respuestas HTTP consistentes.
/// 
/// Mapeo de excepciones:
/// - ConcurrencyException  -> 409 Conflict
/// - ArgumentException    -> 400 Bad Request
/// - Exception (genérica) -> 500 Internal Server Error
/// 
/// Todas las respuestas de error se devuelven en formato JSON e incluyen un traceId
/// para facilitar la trazabilidad y el diagnóstico.
/// </remarks>

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }


    /// <summary>
    /// Ejecuta el pipeline HTTP y captura excepciones no manejadas.
    /// </summary>
    /// 
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Conflicto de concurrencia detectado");
            await WriteError(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Solicitud inválida");
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Excepción no controlada");
            await WriteError(context, HttpStatusCode.InternalServerError, "Ocurrió un error interno en el servidor");
        }
    }


    /// <summary>
    /// Escribe una respuesta de error en formato JSON.
    /// </summary>

    private static Task WriteError(HttpContext context, HttpStatusCode status, string message)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = context.Response.StatusCode,
            error = message,
            traceId = context.TraceIdentifier
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
