using Desafio.Application.Models;
using Desafio.Application.Validators;
using Desafio.Domain.Interfaces;

namespace Desafio.Application.Services;

public class StatusService : IStatusService
{
    private readonly IPedidoRepository _repository;
    private readonly IStatusValidator _validator;

    public StatusService(
        IPedidoRepository repository,
        IStatusValidator validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<StatusResponse> ProcessarStatusAsync(StatusRequest request)
    {
        var pedido = await _repository.ObterPorNumeroAsync(request.Pedido);

        var statusList = _validator.Validar(request, pedido);

        return new StatusResponse
        {
            Pedido = request.Pedido,
            Status = statusList
        };
    }
}