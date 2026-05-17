using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;
using ComercialMorro.API.Repositories.Interfaces;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _repository;

        public CategoriaService(ICategoriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoriaDto>> GetAllAsync()
        {
            var categorias = await _repository.GetAllAsync();
            return categorias.Select(c => new CategoriaDto
            {
                IdCategoria = c.IdCategoria,
                Descricao = c.Descricao
            });
        }

        public async Task<CategoriaDto?> GetByIdAsync(int id)
        {
            var categoria = await _repository.GetByIdAsync(id);
            if (categoria == null) return null;

            return new CategoriaDto
            {
                IdCategoria = categoria.IdCategoria,
                Descricao = categoria.Descricao
            };
        }

        public async Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descricao))
                throw new Exception("Descrição da categoria é obrigatória.");

            var categoria = new Categoria
            {
                Descricao = dto.Descricao.Trim()
            };

            await _repository.AddAsync(categoria);
            return await GetByIdAsync(categoria.IdCategoria)
                ?? throw new Exception("Erro ao criar categoria.");
        }

        public async Task UpdateAsync(int id, CategoriaCreateDto dto)
        {
            var categoria = await _repository.GetByIdAsync(id);
            if (categoria == null)
                throw new Exception("Categoria não encontrada.");

            categoria.Descricao = dto.Descricao.Trim();
            await _repository.UpdateAsync(categoria);
        }

        public async Task DeleteAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new Exception("Categoria não encontrada.");

            await _repository.DeleteAsync(id);
        }
    }
}