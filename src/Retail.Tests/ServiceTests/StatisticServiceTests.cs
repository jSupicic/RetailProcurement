using AutoMapper;
using FluentAssertions;
using Moq;
using Retail.Application.DTOs;
using Retail.Application.Services;
using Retail.Domain.Entities;
using Retail.Infrastructure.Repositories;
using System.Linq.Expressions;
using Xunit;

namespace Retail.Tests.ServiceTests
{
    public class StatisticsServiceTests
    {
        private readonly Mock<IRepository<Supplier>> _supplierMockRepo;
        private readonly Mock<IRepository<SupplierStoreItem>> _supplierStoreItemMockRepo;
        private readonly Mock<IRepository<QuarterlyPlan>> _quarterlyPlanMockRepo;
        private readonly StatisticsService _statisticsService;
        private readonly IMapper _mapper;

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
        public async Task GetSupplierStatisticsAsync_ShouldReturnDto_WhenSupplierExists()
        {
            var supplier = new Supplier { Id = 1, Name = "Supplier A" };
            _supplierMockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(supplier);

            var result = await _statisticsService.GetSupplierStatisticsAsync(1);

            result.Should().NotBeNull();
            result!.SupplierId.Should().Be(1);
            result.SupplierName.Should().Be("Supplier A");
        }

        [Fact]
        public async Task GetSupplierStatisticsAsync_ShouldReturnNull_WhenSupplierDoesNotExist()
        {
            _supplierMockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Supplier?)null);

            var result = await _statisticsService.GetSupplierStatisticsAsync(99);

            result.Should().BeNull();
        }

        /*
        [Fact]
        public async Task GetBestOfferAsync_ShouldReturnBestOffer_WhenItemsExist()
        {
            var items = new List<SupplierStoreItem>
            {
                new SupplierStoreItem { SupplierId = 1, StoreItemId = 10, SupplierPrice = 200, Supplier = new Supplier { Id = 1, Name = "A" } },
                new SupplierStoreItem { SupplierId = 2, StoreItemId = 10, SupplierPrice = 150, Supplier = new Supplier { Id = 2, Name = "B" } },
                new SupplierStoreItem { SupplierId = 3, StoreItemId = 11, SupplierPrice = 100 }
            };

            _supplierStoreItemMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(items);

            var result = await _statisticsService.GetBestOfferAsync(10);

            result.Should().NotBeNull();
            result!.SupplierName.Should().Be("B"); // lowest price
            result.StoreItemId.Should().Be(10);
            result.SupplierPrice.Should().Be(150);
        }
        */
        [Fact]
        public async Task GetBestOfferAsync_ShouldReturnNull_WhenNoItemsFound()
        {
            _supplierStoreItemMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<SupplierStoreItem>());

            var result = await _statisticsService.GetBestOfferAsync(99);

            result.Should().BeNull();
        }
        /*
        [Fact]
        public async Task CreateQuarterlyPlanAsync_ShouldAddNewPlan_WhenNotExists()
        {
            var dto = new QuarterlyPlanCreateDto { Year = 2025, Quarter = 3, SupplierIds = new int[] { 1, 2 } };
            var suppliers = new List<Supplier> { new Supplier { Id = 1 }, new Supplier { Id = 2 } };

            _supplierMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(suppliers);
            _quarterlyPlanMockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<QuarterlyPlan, bool>>>()))
                                  .ReturnsAsync(new List<QuarterlyPlan>());

            _quarterlyPlanMockRepo.Setup(r => r.AddAsync(It.IsAny<QuarterlyPlan>())).Returns(Task.CompletedTask);
            _quarterlyPlanMockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _statisticsService.CreateQuarterlyPlanAsync(dto);

            result.Should().BeTrue();
            _quarterlyPlanMockRepo.Verify(r => r.AddAsync(It.IsAny<QuarterlyPlan>()), Times.Once);
            _quarterlyPlanMockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
        */

        [Fact]
        public async Task GetCurrentQuarterPlanAsync_ShouldReturnSuppliers_WhenPlanExists()
        {
            var now = DateTime.UtcNow;
            int quarter = (now.Month - 1) / 3 + 1;

            var plan = new QuarterlyPlan { Year = now.Year, Quarter = quarter, SupplierIds = new int[] { 1, 2 } };
            _quarterlyPlanMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuarterlyPlan> { plan });

            var suppliers = new List<Supplier>
            {
                new Supplier { Id = 1, Name = "A" },
                new Supplier { Id = 2, Name = "B" }
            };
            _supplierMockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
                             .ReturnsAsync(suppliers);

            var result = await _statisticsService.GetCurrentQuarterPlanAsync();

            result.Should().NotBeNull();
            result!.Should().HaveCount(2);
            result.Select(s => s.Id).Should().Contain(new[] { 1, 2 });
        }

        [Fact]
        public async Task GetCurrentQuarterPlanAsync_ShouldReturnNull_WhenNoPlanExists()
        {
            _quarterlyPlanMockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<QuarterlyPlan>());

            var result = await _statisticsService.GetCurrentQuarterPlanAsync();

            result.Should().BeNull();
        }
    }
}
