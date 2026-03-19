namespace Desafio.Application.Models;

public class PedidoRequest
{
    public string Pedido { get; set; } = string.Empty;
    public List<ItemPedidoModel> Itens { get; set; } = new();
}

public class ItemPedidoModel
{
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoUnitario { get; set; }
    public int Qtd { get; set; }
}