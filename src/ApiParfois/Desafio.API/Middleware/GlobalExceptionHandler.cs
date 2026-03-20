using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log do erro
        _logger.LogError(
            exception,
            "Erro não tratado: {Message} | Path: {Path}",
            exception.Message,
            httpContext.Request.Path
        );

        // Determina status code e título baseado no tipo de exceção
        var (statusCode, title, detail) = exception switch
        {
            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                "Requisição Inválida",
                argEx.Message
            ),

            InvalidOperationException invEx => (
                StatusCodes.Status400BadRequest,
                "Operação Inválida",
                invEx.Message
            ),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso Não Encontrado",
                "O recurso solicitado não foi encontrado"
            ),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "Não Autorizado",
                "Você não tem permissão para acessar este recurso"
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro Interno do Servidor",
                "Ocorreu um erro inesperado. Tente novamente mais tarde."
            )
        };

        // Cria o ProblemDetails (padrão RFC 7807)
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        // Adiciona informações extras em desenvolvimento
        if (httpContext.RequestServices
            .GetRequiredService<IHostEnvironment>()
            .IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        // Define o status code da resposta
        httpContext.Response.StatusCode = statusCode;

        // Retorna o JSON
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken
        );

        return true; // Indica que a exceção foi tratada
    }
}