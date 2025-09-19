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
        private readonly Mock<IRepository<Supplier>> _supplierMockRepo;
        private readonly Mock<IRepository<SupplierStoreItem>> _supplierStoreItemMockRepo;
        private readonly Mock<IRepository<StoreItem>> _storeItemMockRepo;
        private readonly SupplierStoreItemService _supplierStoreItemService;

        public SupplierStoreItemServiceTests()
        {
            _supplierMockRepo = new Mock<IRepository<Supplier>>();
            _supplierStoreItemMockRepo = new Mock<IRepository<SupplierStoreItem>>();
            _storeItemMockRepo = new Mock<IRepository<StoreItem>>();

            _supplierStoreItemService = new SupplierStoreItemService(
                _supplierStoreItemMockRepo.Object,
                _storeItemMockRepo.Object,
                _supplierMockRepo.Object,
                _mapper
            );
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
            var result = await _supplierStoreItemService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().SupplierId.Should().Be(1);
            result.First().StoreItemId.Should().Be(10);

            _supplierStoreItemMockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            var entity = new SupplierStoreItem { SupplierId = 1, StoreItemId = 10, SupplierPrice = 100 };

            _supplierStoreItemMockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<SupplierStoreItem, bool>>>()))
                     .ReturnsAsync(new List<SupplierStoreItem> { entity });

            // Act
            var result = await _supplierStoreItemService.DeleteAsync(1, 10);

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
            var result = await _supplierStoreItemService.DeleteAsync(1, 10);

            // Assert
            result.Should().BeFalse();

            _supplierStoreItemMockRepo.Verify(r => r.DeleteAsync(It.IsAny<SupplierStoreItem>()), Times.Never);
            _supplierStoreItemMockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
