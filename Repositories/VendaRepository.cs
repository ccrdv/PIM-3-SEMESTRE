using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Data;
using ComercialMorro.API.Models;
using ComercialMorro.API.Repositories.Interfaces;

namespace ComercialMorro.API.Repositories
{
    public class VendaRepository : IVendaRepository
    {
        private readonly ApplicationDbContext _context;

        public VendaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Venda> AddAsync(Venda venda)
        {
            await _context.Vendas.AddAsync(venda);
            await _context.SaveChangesAsync();
            return venda;
        }

        public async Task<Venda?> GetByIdAsync(int id)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .Include(v => v.Cliente)
                .FirstOrDefaultAsync(v => v.IdVenda == id);
        }

        public async Task<IEnumerable<Venda>> GetAllAsync()
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.DataHora)
                .ToListAsync();
        }
    }
}