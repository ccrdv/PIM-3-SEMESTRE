using ComercialMorro.API.Models;

namespace ComercialMorro.API.Repositories.Interfaces
{
    public interface IVendaRepository
    {
        Task<Venda> AddAsync(Venda venda);
        Task<Venda?> GetByIdAsync(int id);
        Task<IEnumerable<Venda>> GetAllAsync();
    }
}