using Desafio.Domain.Entities;

namespace Desafio.Domain.Interfaces;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorNumeroAsync(string numeroPedido);
    Task<Pedido?> ObterPorIdAsync(int id);
    Task<IEnumerable<Pedido>> ObterTodosAsync();
    Task AdicionarAsync(Pedido pedido);
    Task AtualizarAsync(Pedido pedido);
    Task RemoverAsync(int id);
    Task<bool> ExisteAsync(string numeroPedido);
    Task<Pedido?> ObterPorIdComItensAsync(int id);
    Task SalvarAlteracoesAsync();
}