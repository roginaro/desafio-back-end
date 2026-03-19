using Desafio.Application.Models;

namespace Desafio.Application.Services;

public interface IStatusService
{
    Task<StatusResponse> ProcessarStatusAsync(StatusRequest request);
}