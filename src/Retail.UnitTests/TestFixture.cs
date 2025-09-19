using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Retail.Infrastructure.Context;
using Xunit;
using Retail.Application.Services;
using Retail.Application.Mappings;
using Retail.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Retail.UnitTests
{
    public class TestFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public TestFixture()
        {
            var services = new ServiceCollection();

            services.AddDbContext<RetailDbContext>(options => options.UseInMemoryDatabase("TestDatabase"));
            services.AddScoped<RetailDbContext>();

            var mapper = new MapperConfiguration(configuration =>
            {
                configuration.AddProfile(new MappingProfile());
            }).CreateMapper();
            services.AddScoped(x => mapper);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IStoreItemService, StoreItemService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierStoreItemService, SupplierStoreItemService>();
            services.AddScoped<IStatisticsService, StatisticsService>();

            ServiceProvider = services.BuildServiceProvider();

            //SeedMockData().Wait();
        }

        //SeedMockData()

        public void Dispose()
        {
            ServiceProvider.GetRequiredService<RetailDbContext>().Dispose();
        }
    }

    [CollectionDefinition("Retail Tests")]
    public class TestFixtureCollection : ICollectionFixture<TestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
