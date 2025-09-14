using AutoMapper;
using Retail.Application.DTOs;
using Retail.Domain.Entities;

namespace Retail.Application.Mappings;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<StoreItem, StoreItemDto>();
        CreateMap<StoreItemCreateDto, StoreItem>();
        CreateMap<StoreItemUpdateDto, StoreItem>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
