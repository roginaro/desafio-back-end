namespace Desafio.Domain.Entities;

public class ItemPedido:Entity
{
    public string Descricao { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public int Quantidade { get; private set; }
    public int PedidoId { get; private set; }

    // Construtor para EF Core
    public ItemPedido() 
    { 
        Descricao = string.Empty;
    }

    public ItemPedido(string descricao, decimal precoUnitario, int quantidade)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição não pode ser vazia", nameof(descricao));

        if (precoUnitario <= 0)
            throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));

        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

        Descricao = descricao;
        PrecoUnitario = precoUnitario;
        Quantidade = quantidade;
    }

    public decimal ObterValorTotal()
    {
        return PrecoUnitario * Quantidade;
    }
}