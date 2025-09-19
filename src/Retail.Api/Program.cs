using Microsoft.EntityFrameworkCore;
using Retail.Application.Mappings;
using Retail.Application.Services;
using Retail.Infrastructure.Context;
using Retail.Infrastructure.Repositories;
using Retail.Infrastructure.Seed;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Locate XML file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Optional: Customize Swagger info
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Retail Procurement System API",
        Version = "v1",
        Description = "API for managing store items, suppliers, and procurement statistics."
    });
});

// SignalR + CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b => b
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(_ => true)); // permissive for development
});
builder.Services.AddSignalR();

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
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retail API V1");
    });
}
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RetailDbContext>();
    db.Database.Migrate(); // Apply migrations
    SeedData.SeedDatabase(db);
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { } // For integration testing