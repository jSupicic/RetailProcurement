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
                src.SupplierStoreItems.Select(ssi => new SupplierStoreItemDto
                {
                    Name = ssi.Supplier.Name,
                    Price = ssi.SupplierPrice
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
    }
}