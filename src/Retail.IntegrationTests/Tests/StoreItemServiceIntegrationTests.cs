using FluentAssertions;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Xunit;

namespace Retail.IntegrationTests.Tests;

public class StoreItemServiceIntegrationTests : IntegrationTestBase
{
    private readonly IStoreItemService _service;

    public StoreItemServiceIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _service = GetService<IStoreItemService>();
    }

    [Fact]
    public async Task StoreItem_CRUD_Flow_Works()
    {
        // Create
        var createDto = new StoreItemCreateDto
        {
            Name = "Integration Laptop",
            Description = "Integration test laptop",
            StockQuantity = 7
        };

        var created = await _service.CreateAsync(createDto);

        created.Should().NotBeNull();
        created.Name.Should().Be("Integration Laptop");
        created.Description.Should().Be("Integration test laptop");
        created.StockQuantity.Should().Be(7);

        // Read
        var fetched = await _service.GetByIdAsync(created.Id);
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("Integration Laptop");

        // Update
        var updateDto = new StoreItemUpdateDto
        {
            Name = "Updated Laptop",
            Description = "Updated desc",
            StockQuantity = 11
        };

        var updateResult = await _service.UpdateAsync(created.Id, updateDto);
        updateResult.Should().BeTrue();

        var afterUpdate = await _service.GetByIdAsync(created.Id);
        afterUpdate.Should().NotBeNull();
        afterUpdate!.Name.Should().Be("Updated Laptop");
        afterUpdate.Description.Should().Be("Updated desc");
        afterUpdate.StockQuantity.Should().Be(11);

        // Delete
        var deleteResult = await _service.DeleteAsync(created.Id);
        deleteResult.Should().BeTrue();

        var afterDelete = await _service.GetByIdAsync(created.Id);
        afterDelete.Should().BeNull();
    }
}