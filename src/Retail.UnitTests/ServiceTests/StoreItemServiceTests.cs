using FluentAssertions;
using Moq;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Retail.UnitTests.ServiceTests
{
    public class StoreItemServiceTests : TestBase
    {
        private readonly Mock<IRepository<StoreItem>> _storeItemMockRepo;
        private readonly StoreItemService _service;

        public StoreItemServiceTests()
        {
            _storeItemMockRepo = new Mock<IRepository<StoreItem>>();
            _service = new StoreItemService(_storeItemMockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _storeItemMockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((StoreItem?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDto_WhenItemExists()
        {
            var storeItem = new StoreItem
            {
                Id = 1,
                Name = "Keyboard",
                Description = "Mechanical Keyboard",
                StockQuantity = 10,
                SupplierStoreItems = new List<SupplierStoreItem>
                {
                    new SupplierStoreItem
                    {
                        SupplierId = 200,
                        SupplierPrice = 80,
                        Supplier = new Supplier { Id = 200, Name = "Peripheral Supplier" }
                    }
                }
            };

            _storeItemMockRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(storeItem);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(1);
            result.Name.Should().Be("Keyboard");
            result.Suppliers.Should().ContainSingle();
            result.Suppliers!.First().Name.Should().Be("Peripheral Supplier");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenItemDoesNotExist()
        {
            // Arrange
            _storeItemMockRepo
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((StoreItem?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedItem()
        {
            var dto = new StoreItemCreateDto { Name = "Laptop", Description = "Expensive", StockQuantity = 21};

            _storeItemMockRepo.Setup(r => r.AddAsync(It.IsAny<StoreItem>())).Returns(Task.CompletedTask);
            _storeItemMockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.Equal("Laptop", result.Name);
            Assert.Equal("Expensive", result.Description);
            Assert.Equal(21, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            var entity = new StoreItem
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description",
                StockQuantity = 5
            };

            var dto = new StoreItemUpdateDto
            {
                Name = "New Name",
                Description = "New Description",
                StockQuantity = 10
            };

            _storeItemMockRepo
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(entity);

            _storeItemMockRepo
                .Setup(r => r.UpdateAsync(entity))
                .Returns(Task.CompletedTask);

            _storeItemMockRepo
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            result.Should().BeTrue();
            entity.Name.Should().Be("New Name");
            entity.Description.Should().Be("New Description");
            entity.StockQuantity.Should().Be(10);

            _storeItemMockRepo.Verify(r => r.UpdateAsync(entity), Times.Once);
            _storeItemMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            _storeItemMockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((StoreItem?)null);

            var result = await _service.UpdateAsync(1, new StoreItemUpdateDto { Name = "Updated", StockQuantity = 32 });

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
        {
            var item = new StoreItem { Id = 1, Name = "Phone" };
            _storeItemMockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
            _storeItemMockRepo.Verify(r => r.DeleteAsync(item), Times.Once);
            _storeItemMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
