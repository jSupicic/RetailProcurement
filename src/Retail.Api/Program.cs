using Microsoft.EntityFrameworkCore;
using Retail.Application.Services;
using Retail.Infrastructure.Context;
using Bogus;
using Retail.Infrastructure.Seed;
using Retail.Infrastructure.Repositories;
using Retail.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core: configure connection string (replace with your local)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<RetailDbContext>(opt => opt
    .UseLazyLoadingProxies()
    .UseNpgsql(conn, b => b.MigrationsAssembly("Retail.Infrastructure")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register services / repositories
builder.Services.AddScoped<IStoreItemService, StoreItemService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<ISupplierStoreItemService, SupplierStoreItemService>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RetailDbContext>();
    db.Database.Migrate(); // Apply migrations
    //SeedData.SeedDatabase(db);
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();

public partial class Program { } // For integration testing