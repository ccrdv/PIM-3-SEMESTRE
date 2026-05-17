using ComercialMorro.API.DTOs;

namespace ComercialMorro.API.Services.Interfaces
{
    public interface IPessoaService
    {
        Task<PessoaResponseDto> CreateAsync(PessoaCreateDto dto);
        Task<PessoaResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<PessoaResponseDto>> GetAllAsync();
    }
}