using AutoMapper;
using ProductMgt.Domain;
using ProductMgtServices.Dtos;

namespace ProductMgtServices.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductResponseItem>()
                .ForMember(x => x.Name, src => src.MapFrom(opt => opt.Name));
        }
    }
}