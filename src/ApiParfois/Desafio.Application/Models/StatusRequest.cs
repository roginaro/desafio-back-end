namespace Desafio.Application.Models;

public class StatusRequest
{
    public string Status { get; set; } = string.Empty;
    public int ItensAprovados { get; set; }
    public decimal ValorAprovado { get; set; }
    public string Pedido { get; set; } = string.Empty;
}