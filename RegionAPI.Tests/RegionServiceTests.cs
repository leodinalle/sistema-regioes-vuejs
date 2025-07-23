using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RegionAPI.Data;
using RegionAPI.Model;
using RegionAPI.ViewModels;
using RegionAPI.Service;
using Xunit;
using Microsoft.EntityFrameworkCore;


namespace RegionAPI.Tests
{
    public class RegionServiceTests
    {
        private RegionService CreatedService(out AppDbContext context)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            context = new AppDbContext(options);
            return new RegionService(context);
        }

        [Fact]
        public async Task ShouldCreateValidRegion()
        {
            var service = CreatedService(out var context);

            var novaRegiao = new RegionInputModel { Uf = "São Paulo", Nome = "Zona Sul" };
            var (sucesso, erro) = await service.CreatedRegion(novaRegiao);

            Assert.True(sucesso);
            Assert.Null(erro);
            Assert.Single(context.Regions);
        }

        [Fact]
        public async Task ShouldNotCreateDuplicateRegion()
        {
            var service = CreatedService(out var context);

            await service.CreatedRegion(new RegionInputModel { Uf = "Minas Gerais", Nome = "Interior" });
            var (sucesso, erro) = await service.CreatedRegion(new RegionInputModel { Uf = "Minas Gerais", Nome = "Interior" });

            Assert.False(sucesso);
            Assert.Equal("existe uma região com esse nome e esse UF", erro);
        }

        [Fact]
        public async Task ShouldUpdateRegion()
        {
            var service = CreatedService(out var context);

            var created = new RegionInputModel { Uf = "RJ", Nome = "Capital" };
            await service.CreatedRegion(created);
            var saved = context.Regions.First();

            var input = new RegionInputModel
            {
                Id = saved.Id,
                Uf = "RJ",
                Nome = "Centro",
                Ativo = saved.Ativo
            };

            var (sucesso, erro) = await service.UpdatedRegion(input);
            Assert.True(sucesso);
            Assert.Equal("Centro", context.Regions.First().Nome);
        }


        [Fact]
        public async Task ShouldInactivateAndActivateRegion()
        {
            var service = CreatedService(out var context);
            var regiao = new RegionInputModel { Uf = "Bahia", Nome = "Salvador" };
            await service.CreatedRegion(regiao);
            var id = context.Regions.First().Id;

            var inativado = await service.InactivateRegion(id);
            Assert.True(inativado);
            Assert.False(context.Regions.First().Ativo);

            var ativado = await service.ActiveRegion(id);
            Assert.True(ativado);
            Assert.True(context.Regions.First().Ativo);
        }

        [Fact]
        public async Task ShouldReturnOrderedRegions()
        {
            var service = CreatedService(out var context);
            await service.CreatedRegion(new RegionInputModel { Uf = "São Paulo", Nome = "Zona Norte" });
            await service.CreatedRegion(new RegionInputModel { Uf = "Minas Gerais", Nome = "Belo Horizonte" });

            var lista = await service.ListRegion();

            Assert.Equal("Minas Gerais", lista[0].Uf);
            Assert.Equal("São Paulo", lista[1].Uf);
        }
    }
}
