using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;

namespace ComercialMorro.API.Services.Interfaces
{
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoDto>> GetAllAsync();
        Task<ProdutoDto?> GetByIdAsync(int id);
        Task<ProdutoDto> CreateAsync(ProdutoCreateDto dto);
        Task UpdateAsync(int id, ProdutoCreateDto dto);
        Task DeleteAsync(int id);
    }
}