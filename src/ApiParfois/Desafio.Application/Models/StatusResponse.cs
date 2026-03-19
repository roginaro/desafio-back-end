namespace Desafio.Application.Models;

public class StatusResponse
{
    public string Pedido { get; set; } = string.Empty;
    public List<string> Status { get; set; } = new();
}