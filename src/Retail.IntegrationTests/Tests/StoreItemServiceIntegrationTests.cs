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

    [Fact]
    public async Task GetAllAsync_ShouldContainCreatedItems()
    {
        // Arrange - create two unique store items
        var name1 = "Integration_GetAll_1_" + Guid.NewGuid().ToString("N");
        var name2 = "Integration_GetAll_2_" + Guid.NewGuid().ToString("N");

        var dto1 = new StoreItemCreateDto
        {
            Name = name1,
            Description = "desc 1",
            StockQuantity = 5
        };
        var dto2 = new StoreItemCreateDto
        {
            Name = name2,
            Description = "desc 2",
            StockQuantity = 10
        };

        var created1 = await _service.CreateAsync(dto1);
        var created2 = await _service.CreateAsync(dto2);

        // Act
        var all = (await _service.GetAllAsync()).ToList();

        // Assert
        all.Should().NotBeNull();
        all.Select(x => x.Name).Should().Contain(new[] { name1, name2 });
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSeparateEntries_ForMultipleCreates()
    {
        // Arrange - create two items with different data
        var baseName = "Integration_DuplicateCheck_" + Guid.NewGuid().ToString("N");
        var createdA = await _service.CreateAsync(new StoreItemCreateDto { Name = baseName + "_A", Description = "A", StockQuantity = 1 });
        var createdB = await _service.CreateAsync(new StoreItemCreateDto { Name = baseName + "_B", Description = "B", StockQuantity = 2 });

        // Act
        var all = (await _service.GetAllAsync()).ToList();

        // Assert
        all.Should().Contain(i => i.Id == createdA.Id);
        all.Should().Contain(i => i.Id == createdB.Id);
    }

    [Fact]
    public async Task GetAllAsync_NewItem_ShouldHaveNoSuppliersByDefault()
    {
        // Arrange
        var name = "Integration_NoSuppliers_" + Guid.NewGuid().ToString("N");
        var created = await _service.CreateAsync(new StoreItemCreateDto { Name = name, Description = "no suppliers", StockQuantity = 3 });

        // Act
        var all = (await _service.GetAllAsync()).ToList();
        var fetched = all.FirstOrDefault(i => i.Id == created.Id);

        // Assert
        fetched.Should().NotBeNull();
        // Suppliers may be null or an empty list depending on mapping; accept both
        (fetched!.Suppliers == null || !fetched.Suppliers.Any()).Should().BeTrue();
    }
}