using Moq;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Retail.Tests.ServiceTests
{
    public class StoreItemServiceTests : TestBase
    {
        private readonly Mock<IRepository<StoreItem>> _mockRepo;
        private readonly StoreItemService _service;

        public StoreItemServiceTests()
        {
            _mockRepo = new Mock<IRepository<StoreItem>>();
            _service = new StoreItemService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((StoreItem?)null);

            var result = await _service.GetByIdAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreatedItem()
        {
            var dto = new StoreItemCreateDto { Name = "Laptop", Description = "Expensive", StockQuantity = 21};

            _mockRepo.Setup(r => r.AddAsync(It.IsAny<StoreItem>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.Equal("Laptop", result.Name);
            Assert.Equal("Expensive", result.Description);
            Assert.Equal(21, result.StockQuantity);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_WhenNotFound()
        {
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((StoreItem?)null);

            var result = await _service.UpdateAsync(1, new StoreItemUpdateDto { Name = "Updated" });

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsTrue_WhenDeleted()
        {
            var item = new StoreItem { Id = 1, Name = "Phone" };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(item);

            var result = await _service.DeleteAsync(1);

            Assert.True(result);
            _mockRepo.Verify(r => r.DeleteAsync(item), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
