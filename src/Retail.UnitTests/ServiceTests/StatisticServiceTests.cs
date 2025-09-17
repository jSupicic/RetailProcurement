using AutoMapper;
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
    }
}
