using FluentAssertions;
using Moq;
using Retail.Application.Services;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace Retail.UnitTests.ServiceTests
{
    public class SupplierStoreItemServiceTests : TestBase
    {
        private readonly Mock<IRepository<SupplierStoreItem>> _supplierStoreItemMockRepo;
        private readonly SupplierStoreItemService _service;

        public SupplierStoreItemServiceTests()
        {
            _supplierStoreItemMockRepo = new Mock<IRepository<SupplierStoreItem>>();
            _service = new SupplierStoreItemService(_supplierStoreItemMockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_Succes()
        {
            // Arrange
            var entities = new List<SupplierStoreItem>
                {
                    new SupplierStoreItem { SupplierId = 1, StoreItemId = 10, SupplierPrice = 100 },
                    new SupplierStoreItem { SupplierId = 2, StoreItemId = 20, SupplierPrice = 200 }
                };

            _supplierStoreItemMockRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().SupplierId.Should().Be(1);
            result.First().StoreItemId.Should().Be(10);

            _supplierStoreItemMockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_OneSuccess_SecondSameFail()
        {
            // Arrange
            var dto = new SupplierStoreItemCreateDto { SupplierId = 1, StoreItemId = 5, SupplierPrice = 50 };
            SupplierStoreItem? capturedEntity = null;
            var callCount = 0;

            _supplierStoreItemMockRepo.Setup(r => r.AddAsync(It.IsAny<SupplierStoreItem>()))
                     .Callback<SupplierStoreItem>(entity =>
                     {
                         callCount++;
                         if (callCount == 1)
                         {
                             capturedEntity = entity;
                         }
                         if (callCount > 1)
                         {
                             throw new InvalidOperationException("Entity already exists");
                         }
                     })
                     .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync(dto);
            Func<Task> result2 = async () => await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.SupplierId.Should().Be(1);
            result.StoreItemId.Should().Be(5);
            result.SupplierPrice.Should().Be(50);

            await result2.Should().ThrowAsync<InvalidOperationException>();

            capturedEntity.Should().NotBeNull();
            capturedEntity!.SupplierId.Should().Be(1);

            _supplierStoreItemMockRepo.Verify(r => r.AddAsync(It.IsAny<SupplierStoreItem>()), Times.Exactly(2));
            _supplierStoreItemMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            var entity = new SupplierStoreItem { SupplierId = 1, StoreItemId = 10, SupplierPrice = 100 };

            _supplierStoreItemMockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<SupplierStoreItem, bool>>>()))
                     .ReturnsAsync(new List<SupplierStoreItem> { entity });

            // Act
            var result = await _service.DeleteAsync(1, 10);

            // Assert
            result.Should().BeTrue();

            _supplierStoreItemMockRepo.Verify(r => r.DeleteAsync(entity), Times.Once);
            _supplierStoreItemMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            // Arrange
            _supplierStoreItemMockRepo
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<SupplierStoreItem, bool>>>()))
                .ReturnsAsync(new List<SupplierStoreItem>());


            // Act
            var result = await _service.DeleteAsync(1, 10);

            // Assert
            result.Should().BeFalse();

            _supplierStoreItemMockRepo.Verify(r => r.DeleteAsync(It.IsAny<SupplierStoreItem>()), Times.Never);
            _supplierStoreItemMockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
