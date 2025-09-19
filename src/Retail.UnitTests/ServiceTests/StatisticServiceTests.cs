using FluentAssertions;
using Moq;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace Retail.UnitTests.ServiceTests
{
    public class StatisticsServiceTests : TestBase
    {
        private readonly Mock<IRepository<Supplier>> _supplierMockRepo;
        private readonly Mock<IRepository<SupplierStoreItem>> _supplierStoreItemMockRepo;
        private readonly Mock<IRepository<QuarterlyPlan>> _quarterlyPlanMockRepo;
        private readonly StatisticsService _statisticsService;

        public StatisticsServiceTests()
        {
            _supplierMockRepo= new Mock<IRepository<Supplier>>();
            _supplierStoreItemMockRepo= new Mock<IRepository<SupplierStoreItem>>();
            _quarterlyPlanMockRepo= new Mock<IRepository<QuarterlyPlan>>();

            _statisticsService = new StatisticsService(
                _supplierMockRepo.Object,
                _supplierStoreItemMockRepo.Object,
                _quarterlyPlanMockRepo.Object,
                _mapper
            );
        }

        [Fact]
        public async Task GetSupplierStatisticsAsync_ShouldReturnNull_WhenSupplierDoesNotExist()
        {
            _supplierMockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            var result = await _statisticsService.GetSupplierStatisticsAsync(99);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetBestOfferAsync_ShouldReturnNull_WhenNoItemsFound()
        {
            _supplierStoreItemMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SupplierStoreItem>());

            var result = await _statisticsService.GetBestOfferAsync(99);

            result.Should().BeNull();
        }
        
        [Fact]
        public async Task GetCurrentQuarterPlanAsync_ShouldReturnNull_WhenNoPlanExists()
        {
            _quarterlyPlanMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuarterlyPlan>());

            var result = await _statisticsService.GetCurrentQuarterPlanAsync();

            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateQuarterlyPlanAsync_ShouldThrow_WhenNoValidSuppliers()
        {
            // Arrange
            _supplierMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Supplier>()); // no suppliers exist

            var dto = new QuarterlyPlanCreateDto
            {
                Year = 2025,
                Quarter = 2,
                SupplierIds = new[] { 1, 2 }
            };

            // Act
            Func<Task> act = async () => await _statisticsService.CreateQuarterlyPlanAsync(dto);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("There are no suppliers with selected Id");
            
            _quarterlyPlanMockRepo.Verify(r => r.AddAsync(It.IsAny<QuarterlyPlan>()), Times.Never);
            _quarterlyPlanMockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateQuarterlyPlanAsync_ShouldAddNewPlan_WhenNoExistingPlan()
        {
            // Arrange
            var existingSuppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "S1" },
                new Supplier { Id = 2, Name = "S2" }
            };
            _supplierMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(existingSuppliers);

            _quarterlyPlanMockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<QuarterlyPlan, bool>>>()))
                .ReturnsAsync(new List<QuarterlyPlan>()); // no existing plan

            QuarterlyPlan? capturedPlan = null;
            _quarterlyPlanMockRepo.Setup(r => r.AddAsync(It.IsAny<QuarterlyPlan>()))
                .Callback<QuarterlyPlan>(p => capturedPlan = p)
                .Returns(Task.CompletedTask);

            _quarterlyPlanMockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new QuarterlyPlanCreateDto
            {
                Year = 2025,
                Quarter = 3,
                SupplierIds = new[] { 1, 2, 99 } // 99 will be filtered out
            };

            // Act
            var result = await _statisticsService.CreateQuarterlyPlanAsync(dto);

            // Assert
            result.Should().BeTrue();
            capturedPlan.Should().NotBeNull();
            capturedPlan!.Year.Should().Be(2025);
            capturedPlan.Quarter.Should().Be(3);
            capturedPlan.Suppliers.Should().NotBeNull();
            capturedPlan.Suppliers.Select(s => s.SupplierId).Should().BeEquivalentTo(new[] { 1, 2 });

            _quarterlyPlanMockRepo.Verify(r => r.AddAsync(It.IsAny<QuarterlyPlan>()), Times.Once);
            _quarterlyPlanMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateQuarterlyPlanAsync_ShouldUpdateExistingPlan_WhenPlanExists()
        {
            // Arrange
            var existingSuppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "S1" },
                new Supplier { Id = 2, Name = "S2" }
            };
            _supplierMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(existingSuppliers);

            var foundPlan = new QuarterlyPlan
            {
                Id = 5,
                Year = 2024,
                Quarter = 1,
                Suppliers = new List<QuarterlyPlanSupplier>()
            };

            _quarterlyPlanMockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<QuarterlyPlan, bool>>>()))
                .ReturnsAsync(new List<QuarterlyPlan> { foundPlan });

            // GetByIdAsync should return an entity to be updated (could be the same instance)
            var entityToUpdate = new QuarterlyPlan
            {
                Id = 5,
                Year = 2024,
                Quarter = 1,
                Suppliers = new List<QuarterlyPlanSupplier>()
            };
            _quarterlyPlanMockRepo.Setup(r => r.GetByIdAsync(foundPlan.Id)).ReturnsAsync(entityToUpdate);

            _quarterlyPlanMockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var dto = new QuarterlyPlanCreateDto
            {
                Year = 2026,
                Quarter = 4,
                SupplierIds = new[] { 2, 1, 1 } // duplicates to be deduped and ordered
            };

            // Act
            var result = await _statisticsService.CreateQuarterlyPlanAsync(dto);

            // Assert
            result.Should().BeTrue();
            // entityToUpdate should have been modified by AutoMapper
            entityToUpdate.Year.Should().Be(2026);
            entityToUpdate.Quarter.Should().Be(4);
            entityToUpdate.Suppliers.Select(s => s.SupplierId).Should().BeEquivalentTo(new[] { 1, 2 });

            _quarterlyPlanMockRepo.Verify(r => r.AddAsync(It.IsAny<QuarterlyPlan>()), Times.Never);
            _quarterlyPlanMockRepo.Verify(r => r.GetByIdAsync(foundPlan.Id), Times.Once);
            _quarterlyPlanMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
