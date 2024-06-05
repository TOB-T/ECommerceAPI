using AutoMapper;
using E_Commerce.API.DataModels;
using E_Commerce.API.Dtos;

namespace E_Commerce.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<OrderDetail, OrderDetailDto>().ReverseMap();

            CreateMap<Order, OrderDto>()
                .ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();

        }



    }
}
