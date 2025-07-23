using Microsoft.EntityFrameworkCore;
using RegionAPI.Data;
using RegionAPI.Model;
using RegionAPI.ViewModels;

namespace RegionAPI.Service
{
    public class RegionService
    {
        private readonly AppDbContext _context;

        public RegionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<RegionViewModel>> ListRegion()
        {
            return await _context.Regions
                .OrderBy(r => r.Uf)
                .ThenBy(r => r.Nome)
                .Select(r => new RegionViewModel
                {
                    Id = r.Id,
                    Uf = r.Uf,
                    Nome = r.Nome,
                    Ativo = r.Ativo
                })
                .ToListAsync();
        }


        public async Task<RegionViewModel?> GetByIdRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null) return null;

            return new RegionViewModel
            {
                Id = region.Id,
                Uf = region.Uf,
                Nome = region.Nome,
                Ativo = region.Ativo
            };
        }

        public async Task<(bool sucess, string error)> CreatedRegion(RegionInputModel region)
        {
            if (await _context.Regions.AnyAsync(r => r.Uf == region.Uf && r.Nome == region.Nome))
            {
                return (false, "existe uma região com esse nome e esse UF");
            }
            var newRegion = new Region
            {
                Uf = region.Uf,
                Nome = region.Nome,
                Ativo = true,
            };

            await _context.Regions.AddAsync(newRegion);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool sucesso, string? erro)> UpdatedRegion(RegionInputModel region)
        {
            if (await _context.Regions.AnyAsync(r => r.Id != region.Id && r.Uf == region.Uf && r.Nome == region.Nome))
                return (false, "Outra região já usa esse nome e UF.");

            var existingRegion = await _context.Regions.FindAsync(region.Id);
            if (existingRegion == null) return (false, "Região não encontrada");

            existingRegion.Uf = region.Uf;
            existingRegion.Nome = region.Nome;

            await _context.SaveChangesAsync();
            return (true, null);
        }


        public async Task<bool> ActiveRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null) return false;

            region.Ativo = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InactivateRegion(int id)
        {
            var region = await _context.Regions.FindAsync(id);
            if (region == null) return false;

            region.Ativo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
