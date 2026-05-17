using ComercialMorro.API.DTOs;

namespace ComercialMorro.API.Services.Interfaces
{
    public interface IVendaService
    {
        Task<VendaResponseDto> RegistrarVendaAsync(NovaVendaRequestDto dto);
        Task<IEnumerable<VendaResponseDto>> GetAllAsync();
        Task<VendaResponseDto?> GetByIdAsync(int id);
    }
}