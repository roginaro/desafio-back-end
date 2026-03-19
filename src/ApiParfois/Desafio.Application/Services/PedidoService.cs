using Desafio.Application.Models;
using Desafio.Domain.Entities;
using Desafio.Domain.Interfaces;

namespace Desafio.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _repository;

    public PedidoService(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task<PedidoResponse?> ObterPorNumeroAsync(string numeroPedido)
    {
        var pedido = await _repository.ObterPorNumeroAsync(numeroPedido);
        return pedido == null ? null : MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse?> ObterPorIdAsync(int id)
    {
        var pedido = await _repository.ObterPorIdAsync(id);
        return pedido == null ? null : MapearParaResponse(pedido);
    }

    public async Task<IEnumerable<PedidoResponse>> ObterTodosAsync()
    {
        var pedidos = await _repository.ObterTodosAsync();
        return pedidos.Select(MapearParaResponse);
    }

    public async Task<PedidoResponse> CriarAsync(PedidoRequest request)
    {
        if (await _repository.ExisteAsync(request.Pedido))
            throw new InvalidOperationException($"Pedido {request.Pedido} já existe");

        var pedido = new Pedido(request.Pedido);

        foreach (var item in request.Itens)
        {
            pedido.AdicionarItem(item.Descricao, item.PrecoUnitario, item.Qtd);
        }

        await _repository.AdicionarAsync(pedido);
        return MapearParaResponse(pedido);
    }

    public async Task<PedidoResponse?> AtualizarAsync(int id, PedidoRequest request)
    {
        var pedidoExistente = await _repository.ObterPorIdAsync(id);
        if (pedidoExistente == null)
            return null;

        // Criar novo pedido com os dados atualizados
        var pedidoAtualizado = new Pedido(request.Pedido);
        
        foreach (var item in request.Itens)
        {
            pedidoAtualizado.AdicionarItem(item.Descricao, item.PrecoUnitario, item.Qtd);
        }

        await _repository.AtualizarAsync(pedidoAtualizado);
        return MapearParaResponse(pedidoAtualizado);
    }

    public async Task<bool> RemoverAsync(int id)
    {
        var pedido = await _repository.ObterPorIdAsync(id);
        if (pedido == null)
            return false;

        await _repository.RemoverAsync(id);
        return true;
    }

    private static PedidoResponse MapearParaResponse(Pedido pedido)
    {
        return new PedidoResponse
        {
            Id = pedido.Id,
            Pedido = pedido.NumeroPedido,
            DataCriacao = pedido.DataCriacao,
            Itens = pedido.Itens.Select(i => new ItemPedidoModel
            {
                Descricao = i.Descricao,
                PrecoUnitario = i.PrecoUnitario,
                Qtd = i.Quantidade
            }).ToList(),
            ValorTotal = pedido.ObterValorTotal(),
            QuantidadeTotalItens = pedido.ObterQuantidadeTotalItens()
        };
    }
}