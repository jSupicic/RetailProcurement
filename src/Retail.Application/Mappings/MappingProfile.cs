using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;

namespace Retail.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // StoreItem: Entity -> DTO
        CreateMap<StoreItem, StoreItemDto>()
            .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src =>
                src.SupplierStoreItems.Select(ssi => new FromSupplierDto
                {
                    Name = ssi.Supplier.Name,
                    Price = ssi.SupplierPrice,
                }).ToList()
            ));

        // StoreItem: DTO -> Entity
        CreateMap<StoreItemCreateDto, StoreItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.SupplierStoreItems, opt => opt.Ignore())
                .ForMember(dest => dest.Sales, opt => opt.Ignore());

        CreateMap<StoreItemUpdateDto, StoreItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Price, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierStoreItems, opt => opt.Ignore())
            .ForMember(dest => dest.Sales, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Supplier: Entity -> DTO
        CreateMap<Supplier, SupplierDto>()
            .ForMember(dest => dest.StoreItems, opt =>
            {
                opt.PreCondition(src => src.SupplierStoreItems != null);
                opt.MapFrom(src =>
                    src.SupplierStoreItems!.Select(ssi => new StoreItemDto
                    {
                        Id = ssi.StoreItemId,
                        Name = ssi.StoreItem.Name,
                        Description = ssi.StoreItem.Description,
                        StockQuantity = ssi.StoreItem.StockQuantity
                    }).ToList()
                );
            });

        // Supplier: DTO -> Entity
        CreateMap<SupplierCreateDto, Supplier>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierStoreItems, opt => opt.Ignore())
            .ForMember(dest => dest.Sales, opt => opt.Ignore())
            .ForMember(dest => dest.QuarterlyPlans, opt => opt.Ignore());

        CreateMap<SupplierUpdateDto, Supplier>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SupplierStoreItems, opt => opt.Ignore())
            .ForMember(dest => dest.Sales, opt => opt.Ignore())
            .ForMember(dest => dest.QuarterlyPlans, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // SupplierStoreItem: Entity -> DTO
        CreateMap<SupplierStoreItem, SupplierStoreItemDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
            .ForMember(dest => dest.StoreItemName, opt => opt.MapFrom(src => src.StoreItem.Name));

        // SupplierStoreItem: DTO -> Entity
        CreateMap<SupplierStoreItemCreateDto, SupplierStoreItem>()
            .ForMember(dest => dest.Supplier, opt => opt.Ignore())
            .ForMember(dest => dest.StoreItem, opt => opt.Ignore());


        //Statistics
        // Supplier → SupplierStatisticDto
        CreateMap<Supplier, SupplierStatisticDto>()
            .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.TotalItemsSold, opt => opt.MapFrom(src => src.Sales.Sum(s => s.Quantity)))
            .ForMember(dest => dest.TotalEarnings, opt => opt.MapFrom(src => src.Sales.Sum(s => s.Quantity * s.StoreItem.Price)));

        // SupplierStoreItem → BestOfferDto
        CreateMap<SupplierStoreItem, SupplierBestOfferDto>()
            .ForMember(dest => dest.StoreItemName, opt => opt.MapFrom(src => src.StoreItem.Name))
            .ForMember(dest => dest.StoreItemPrice, opt => opt.MapFrom(src => src.SupplierPrice))
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name));

        // QuarterlyPlan: Entity -> DTO
        CreateMap<QuarterlyPlan, QuarterlyPlanDto>()
            .ForMember(dest => dest.SupplierIds, opt => opt.MapFrom(src => src.Suppliers.Select(s => s.SupplierId).ToArray()));

        // QuarterlyPlan: DTO -> Entity
        CreateMap<QuarterlyPlanCreateDto, QuarterlyPlan>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src =>
                src.SupplierIds.Select(id => new QuarterlyPlanSupplier { SupplierId = id })));
    }
}