using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Retail.Application.DTOs;
using Xunit;

namespace Retail.IntegrationTests.Tests;

public class StatisticsControllerIntegrationTests : IntegrationTestBase
{
    private readonly HttpClient _client;

    public StatisticsControllerIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSupplierStatistics_ReturnsNotFound_ForUnknownSupplier()
    {
        var response = await _client.GetAsync("/api/statistics/supplier/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateQuarterlyPlan_And_GetCurrentQuarterPlan_Works()
    {
        // Arrange: create two suppliers
        var supplier1 = await _client.PostAsJsonAsync("/api/suppliers", new SupplierCreateDto { Name = "StatSupplier1" });
        var supplier2 = await _client.PostAsJsonAsync("/api/suppliers", new SupplierCreateDto { Name = "StatSupplier2" });
        var s1 = await supplier1.Content.ReadFromJsonAsync<SupplierDto>();
        var s2 = await supplier2.Content.ReadFromJsonAsync<SupplierDto>();

        // Act: create quarterly plan
        var now = System.DateTime.UtcNow;
        var planDto = new QuarterlyPlanCreateDto
        {
            Year = now.Year,
            Quarter = (now.Month - 1) / 3 + 1,
            SupplierIds = new[] { s1!.Id, s2!.Id }
        };
        var createResp = await _client.PostAsJsonAsync("/api/statistics/quarterly-plan", planDto);
        createResp.EnsureSuccessStatusCode();

        // Assert: get current plan
        var getResp = await _client.GetAsync("/api/statistics/quarterly-plan");
        getResp.EnsureSuccessStatusCode();
        var suppliers = await getResp.Content.ReadFromJsonAsync<SupplierDto[]>();
        suppliers.Should().NotBeNull();
        suppliers!.Select(x => x.Name).Should().Contain(new[] { "StatSupplier1", "StatSupplier2" });
    }

    [Fact]
    public async Task GetBestOffer_ReturnsNotFound_WhenNoOffers()
    {
        var response = await _client.GetAsync("/api/statistics/best-offer/99999");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBestOffer_ReturnsLowestPriceOffer_WhenOffersExist()
    {
        // Arrange: create a store item
        var storeResp = await _client.PostAsJsonAsync("/api/storeitems", new StoreItemCreateDto
        {
            Name = "Integration Item",
            Description = "Item for best-offer test",
            StockQuantity = 10
        });
        storeResp.EnsureSuccessStatusCode();
        var storeItem = await storeResp.Content.ReadFromJsonAsync<StoreItemDto>();
        storeItem.Should().NotBeNull();

        // Create two suppliers
        var supplierAResp = await _client.PostAsJsonAsync("/api/suppliers", new SupplierCreateDto { Name = "OfferSupplierA" });
        var supplierBResp = await _client.PostAsJsonAsync("/api/suppliers", new SupplierCreateDto { Name = "OfferSupplierB" });
        var supplierA = await supplierAResp.Content.ReadFromJsonAsync<SupplierDto>();
        var supplierB = await supplierBResp.Content.ReadFromJsonAsync<SupplierDto>();
        supplierA.Should().NotBeNull();
        supplierB.Should().NotBeNull();

        // Create supplier offers with different prices
        var offerAResp = await _client.PostAsJsonAsync("/api/supplierStoreItems", new SupplierStoreItemCreateDto
        {
            SupplierId = supplierA!.Id,
            StoreItemId = storeItem!.Id,
            SupplierPrice = 200m
        });
        offerAResp.EnsureSuccessStatusCode();

        var offerBResp = await _client.PostAsJsonAsync("/api/supplierStoreItems", new SupplierStoreItemCreateDto
        {
            SupplierId = supplierB!.Id,
            StoreItemId = storeItem.Id,
            SupplierPrice = 150m
        });
        offerBResp.EnsureSuccessStatusCode();

        // Act: request best offer
        var bestOfferResp = await _client.GetAsync($"/api/statistics/best-offer/{storeItem.Id}");
        bestOfferResp.EnsureSuccessStatusCode();

        var bestOffer = await bestOfferResp.Content.ReadFromJsonAsync<SupplierBestOfferDto>();

        // Assert: lowest price supplier returned
        bestOffer.Should().NotBeNull();
        bestOffer!.SupplierName.Should().Be("OfferSupplierB");
        bestOffer.StoreItemPrice.Should().Be(150m);
    }
}