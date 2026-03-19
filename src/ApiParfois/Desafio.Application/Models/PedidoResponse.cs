namespace Desafio.Application.Models;

public class PedidoResponse
{
    public int Id { get; set; }
    public string Pedido { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public List<ItemPedidoModel> Itens { get; set; } = new();
    public decimal ValorTotal { get; set; }
    public int QuantidadeTotalItens { get; set; }
}