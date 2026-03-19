using Desafio.Application.Models;
using Desafio.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidoController : ControllerBase
{
    private readonly IPedidoService _pedidoService;

    public PedidoController(IPedidoService pedidoService)
    {
        _pedidoService = pedidoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PedidoResponse>>> ObterTodos()
    {
        var pedidos = await _pedidoService.ObterTodosAsync();
        return Ok(pedidos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PedidoResponse>> ObterPorId(int id)
    {
        var pedido = await _pedidoService.ObterPorIdAsync(id);
        
        if (pedido == null)
            return NotFound(new { message = $"Pedido com ID {id} não encontrado" });

        return Ok(pedido);
    }

    [HttpGet("numero/{numeroPedido}")]
    public async Task<ActionResult<PedidoResponse>> ObterPorNumero(string numeroPedido)
    {
        var pedido = await _pedidoService.ObterPorNumeroAsync(numeroPedido);
        
        if (pedido == null)
            return NotFound(new { message = $"Pedido {numeroPedido} não encontrado" });

        return Ok(pedido);
    }

    [HttpPost]
    public async Task<ActionResult<PedidoResponse>> Criar([FromBody] PedidoRequest request)
    {
        try
        {
            var pedido = await _pedidoService.CriarAsync(request);
            return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PedidoResponse>> Atualizar(int id, [FromBody] PedidoRequest request)
    {
        try
        {
            var pedido = await _pedidoService.AtualizarAsync(id, request);
            
            if (pedido == null)
                return NotFound(new { message = $"Pedido com ID {id} não encontrado" });

            return Ok(pedido);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Remover(int id)
    {
        var removido = await _pedidoService.RemoverAsync(id);
        
        if (!removido)
            return NotFound(new { message = $"Pedido com ID {id} não encontrado" });

        return NoContent();
    }
}