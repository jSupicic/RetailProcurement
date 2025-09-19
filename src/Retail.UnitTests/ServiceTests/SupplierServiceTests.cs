using FluentAssertions;
using Moq;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;
using Xunit;

namespace Retail.UnitTests.ServiceTests
{
    public class SupplierServiceTests : TestBase
    {
        private readonly Mock<IRepository<Supplier>> _supplierMockRepo;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _supplierMockRepo = new Mock<IRepository<Supplier>>();
            _service = new SupplierService(_supplierMockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnMappedSuppliers()
        {
            // Arrange
            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "Supplier A", Email = "a@email.com" },
                new Supplier { Id = 2, Name = "Supplier B", Email = "b@email.com" }
            };

            _supplierMockRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(suppliers);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Should().ContainSingle(s => s.Id == 1 && s.Name == "Supplier A");
            result.Should().ContainSingle(s => s.Id == 2 && s.Name == "Supplier B");

            _supplierMockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMappedSupplier_WhenEntityExists()
        {
            // Arrange
            var supplier = new Supplier { Id = 10, Name = "Test Supplier", Email = "test@email.com" };

            _supplierMockRepo
                .Setup(r => r.GetByIdAsync(10))
                .ReturnsAsync(supplier);

            // Act
            var result = await _service.GetByIdAsync(10);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(10);
            result.Name.Should().Be("Test Supplier");

            _supplierMockRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenEntityDoesNotExist()
        {
            // Arrange
            _supplierMockRepo
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Supplier?)null);

            // Act
            var result = await _service.GetByIdAsync(99);

            // Assert
            result.Should().BeNull();
            _supplierMockRepo.Verify(r => r.GetByIdAsync(99), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedSupplier()
        {
            var dto = new SupplierCreateDto { Name = "Supplier A", Email = "a@mail.com" };

            _supplierMockRepo.Setup(r => r.AddAsync(It.IsAny<Supplier>())).Returns(Task.CompletedTask);
            _supplierMockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.Equal("Supplier A", result.Name);
            Assert.Equal("a@mail.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            _supplierMockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Supplier?)null);

            var result = await _service.UpdateAsync(1, new SupplierUpdateDto { Name = "Updated" });

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            var supplier = new Supplier { Id = 1, Name = "Old Name" };
            var dto = new SupplierUpdateDto { Name = "New Name" };

            _supplierMockRepo.Setup(r => r.GetByIdAsync(1))
                             .ReturnsAsync(supplier);

            // Act
            var result = await _service.UpdateAsync(1, dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("New Name"); // Confirm mapping worked

            _supplierMockRepo.Verify(r => r.UpdateAsync(supplier), Times.Once);
            _supplierMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            var supplier = new Supplier { Id = 5, Name = "To Delete" };

            _supplierMockRepo.Setup(r => r.GetByIdAsync(5))
                             .ReturnsAsync(supplier);

            // Act
            var result = await _service.DeleteAsync(5);

            // Assert
            result.Should().BeTrue();

            _supplierMockRepo.Verify(r => r.DeleteAsync(supplier), Times.Once);
            _supplierMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityDoesNotExist()
        {
            // Arrange
            _supplierMockRepo.Setup(r => r.GetByIdAsync(77))
                             .ReturnsAsync((Supplier?)null);

            // Act
            var result = await _service.DeleteAsync(77);

            // Assert
            result.Should().BeFalse();

            _supplierMockRepo.Verify(r => r.DeleteAsync(It.IsAny<Supplier>()), Times.Never);
            _supplierMockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
