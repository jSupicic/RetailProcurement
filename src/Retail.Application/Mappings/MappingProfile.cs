using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;

namespace Retail.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity -> DTO
        CreateMap<StoreItem, StoreItemDto>()
            .ForMember(dest => dest.Suppliers, opt => opt.MapFrom(src =>
                src.SupplierStoreItems.Select(ssi => new FromSupplierDto
                {
                    Name = ssi.Supplier.Name,
                    Price = ssi.SupplierPrice,
                }).ToList()
            ));

        // DTO -> Entity
        CreateMap<StoreItemCreateDto, StoreItem>();

        CreateMap<StoreItemUpdateDto, StoreItem>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Supplier -> SupplierDto
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

        // DTO -> Entity
        CreateMap<SupplierCreateDto, Supplier>();
        CreateMap<SupplierUpdateDto, Supplier>()
            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) => srcMember != null));

        // Supplier → SupplierStatisticDto (with custom calculations)
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

        // QuarterlyPlan → QuarterlyPlanDto
        CreateMap<QuarterlyPlan, QuarterlyPlanDto>();

        CreateMap<QuarterlyPlanCreateDto, QuarterlyPlan>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));


        CreateMap<SupplierStoreItem, SupplierStoreItemDto>()
            .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
            .ForMember(dest => dest.StoreItemName, opt => opt.MapFrom(src => src.StoreItem.Name));

        CreateMap<SupplierStoreItemCreateDto, SupplierStoreItem>();
    }
}