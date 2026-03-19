using System;
using System.Collections.Generic;
using System.Text;

namespace Desafio.Domain.Entities
{

public class Pedido
{
    public int Id { get; private set; }
    public string NumeroPedido { get; private set; }
    public DateTime DataCriacao { get; private set; }
    
    private readonly List<ItemPedido> _itens;
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    // Construtor para EF Core
    private Pedido() 
    { 
        _itens = new List<ItemPedido>();
        NumeroPedido = string.Empty;
    }

    public Pedido(string numeroPedido)
    {
        if (string.IsNullOrWhiteSpace(numeroPedido))
            throw new ArgumentException("Número do pedido não pode ser vazio", nameof(numeroPedido));

        NumeroPedido = numeroPedido;
        DataCriacao = DateTime.UtcNow;
        _itens = new List<ItemPedido>();
    }

    public void AdicionarItem(string descricao, decimal precoUnitario, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição não pode ser vazia", nameof(descricao));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        var item = new ItemPedido(descricao, precoUnitario, quantidade);
        _itens.Add(item);
    }

    public decimal ObterValorTotal()
    {
        return _itens.Sum(i => i.ObterValorTotal());
    }

    public int ObterQuantidadeTotalItens()
    {
        return _itens.Sum(i => i.Quantidade);
    }
}
}
