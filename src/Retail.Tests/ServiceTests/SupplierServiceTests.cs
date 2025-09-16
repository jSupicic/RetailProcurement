using Moq;
using Xunit;
using Retail.Application.Services;
using Retail.Application.DTOs;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;

namespace Retail.Tests.ServiceTests
{
    public class SupplierServiceTests : TestBase
    {
        private readonly Mock<IRepository<Supplier>> _mockRepo;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _mockRepo = new Mock<IRepository<Supplier>>();
            _service = new SupplierService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Supplier?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedSupplier()
        {
            var dto = new SupplierCreateDto { Name = "Supplier A", Email = "a@mail.com" };

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<Supplier>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.Equal("Supplier A", result.Name);
            Assert.Equal("a@mail.com", result.Email);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Supplier?)null);

            var result = await _service.UpdateAsync(1, new SupplierUpdateDto { Name = "Updated" });

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
        {
            var supplier = new Supplier { Id = 1, Name = "Supplier B" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
            _mockRepo.Verify(r => r.DeleteAsync(supplier), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
