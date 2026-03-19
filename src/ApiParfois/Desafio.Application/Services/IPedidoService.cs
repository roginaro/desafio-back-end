using Desafio.Application.Models;

namespace Desafio.Application.Services;

public interface IPedidoService
{
    Task<PedidoResponse?> ObterPorNumeroAsync(string numeroPedido);
    Task<PedidoResponse?> ObterPorIdAsync(int id);
    Task<IEnumerable<PedidoResponse>> ObterTodosAsync();
    Task<PedidoResponse> CriarAsync(PedidoRequest request);
    Task<PedidoResponse?> AtualizarAsync(int id, PedidoRequest request);
    Task<bool> RemoverAsync(int id);
}