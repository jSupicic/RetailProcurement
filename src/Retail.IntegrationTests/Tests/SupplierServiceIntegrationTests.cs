using FluentAssertions;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Xunit;

namespace Retail.IntegrationTests.Tests;

public class SupplierServiceIntegrationTests : IntegrationTestBase
{
    private readonly ISupplierService _service;

    public SupplierServiceIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _service = GetService<ISupplierService>();
    }

    [Fact]
    public async Task Supplier_CRUD_Flow_Works()
    {
        // Create
        var createDto = new SupplierCreateDto { Name = "IntSupplier", Email = "int@s.com" };
        var created = await _service.CreateAsync(createDto);

        created.Should().NotBeNull();
        created.Name.Should().Be("IntSupplier");
        created.Email.Should().Be("int@s.com");

        // Read all / by id
        var all = await _service.GetAllAsync();
        all.Should().ContainSingle(s => s.Name == "IntSupplier");

        var fetched = await _service.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("IntSupplier");

        // Update
        var updated = await _service.UpdateAsync(created.Id, new SupplierUpdateDto { Name = "UpdatedSupplier", Email = "updated@s.com" });
        updated.Should().BeTrue();

        var afterUpdate = await _service.GetByIdAsync(created.Id);
        afterUpdate.Should().NotBeNull();
        afterUpdate!.Name.Should().Be("UpdatedSupplier");
        afterUpdate.Email.Should().Be("updated@s.com");

        // Delete
        var deleted = await _service.DeleteAsync(created.Id);
        deleted.Should().BeTrue();

        var afterDelete = await _service.GetAllAsync();
        afterDelete.Should().NotContain(s => s.Name == "UpdatedSupplier");
    }
}