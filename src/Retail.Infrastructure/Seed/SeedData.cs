using Bogus;
using Microsoft.EntityFrameworkCore;
using Retail.Domain.Entities;
using Retail.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Retail.Infrastructure.Seed;

public static class SeedData
{
    public static void SeedDatabase(RetailDbContext context)
    {
        if (context.StoreItems.Any() || context.Suppliers.Any())
        {
            Console.WriteLine("Database already seeded.");
            return;
        }

        var random = new Random();

        // 1️⃣ Generate StoreItems
        var storeItemFaker = new Faker<StoreItem>()
            .RuleFor(s => s.Name, f => f.Commerce.ProductName())
            .RuleFor(s => s.Description, f => f.Commerce.ProductDescription())
            .RuleFor(s => s.Price, f => f.Random.Decimal(10, 500))
            .RuleFor(s => s.StockQuantity, f => f.Random.Int(1, 100));

        var storeItems = storeItemFaker.Generate(10);

        // 2️⃣ Generate Suppliers
        var supplierFaker = new Faker<Supplier>()
            .RuleFor(s => s.Name, f => f.Company.CompanyName())
            .RuleFor(s => s.Email, f => f.Internet.Email())
            .RuleFor(s => s.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(s => s.Address, f => f.Address.FullAddress());

        var suppliers = supplierFaker.Generate(5);

        // 3️⃣ Generate SupplierStoreItems (many-to-many)
        var supplierStoreItems = new List<SupplierStoreItem>();
        foreach (var supplier in suppliers)
        {
            var items = storeItems.OrderBy(x => random.Next()).Take(random.Next(2, 6));
            foreach (var item in items)
            {
                var ssi = new SupplierStoreItem
                {
                    Supplier = supplier,
                    StoreItem = item,
                    SupplierPrice = decimal.Round(item.Price * (decimal)(0.8 + random.NextDouble() * 0.4), 2)
                };
                supplier.SupplierStoreItems.Add(ssi);
                item.SupplierStoreItems.Add(ssi);
                supplierStoreItems.Add(ssi);
            }
        }

        // 4️⃣ Generate Sales
        var saleFaker = new Faker<Sale>()
            .RuleFor(s => s.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(s => s.SaleDate, f => f.Date.Recent(30).ToUniversalTime())
            .RuleFor(s => s.StoreItem, f => storeItems[f.Random.Int(0, storeItems.Count - 1)])
            .RuleFor(s => s.Supplier, f => suppliers[f.Random.Int(0, suppliers.Count - 1)]);

        var sales = saleFaker.Generate(20);
        foreach (var sale in sales)
        {
            sale.StoreItem.Sales.Add(sale);
            sale.Supplier.Sales.Add(sale);
        }

        // 5️⃣ Generate QuarterlyPlans
        var quarterlyPlanFaker = new Faker<QuarterlyPlan>()
            .RuleFor(q => q.Year, f => f.Date.Past(2).Year)
            .RuleFor(q => q.Quarter, f => f.Random.Int(1, 4))
            .RuleFor(q => q.CreatedAt, f => f.Date.Recent(60).ToUniversalTime());

        var quarterlyPlans = quarterlyPlanFaker.Generate(3);

        // 7️⃣ Add all to DbContext
        context.StoreItems.AddRange(storeItems);
        context.Suppliers.AddRange(suppliers);
        context.SupplierStoreItems.AddRange(supplierStoreItems);
        context.Sales.AddRange(sales);
        context.QuarterlyPlans.AddRange(quarterlyPlans);

        // 8️⃣ Save to database
        context.SaveChanges();

        Console.WriteLine("Database seeded successfully!");
    }
}
