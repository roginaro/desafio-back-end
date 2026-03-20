using Desafio.Domain.Entities;
using Desafio.Domain.Interfaces;
using Desafio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Desafio.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _context;

    public PedidoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObterPorNumeroAsync(string numeroPedido)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido);
    }

    public async Task<Pedido?> ObterPorIdAsync(int id)
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    //colocar paginação
    public async Task<IEnumerable<Pedido>> ObterTodosAsync()
    {
        return await _context.Pedidos
            .Include(p => p.Itens)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task AdicionarAsync(Pedido pedido)
    {
        await _context.Pedidos.AddAsync(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Pedido pedido)
    {
        _context.Pedidos.Update(pedido);
        await _context.SaveChangesAsync();
    }

    public async Task RemoverAsync(int id)
    {
        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteAsync(string numeroPedido)
    {
        return await _context.Pedidos
            .AnyAsync(p => p.NumeroPedido == numeroPedido);
    }

    public async Task<Pedido?> ObterPorIdComItensAsync(int id)
    {
        IQueryable<Pedido> query = _context.Pedidos
            .Include(p => p.Itens);
        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task SalvarAlteracoesAsync()
    {
        await _context.SaveChangesAsync();
    }
}