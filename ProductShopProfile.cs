using AutoMapper;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {

            //import dtos
            CreateMap<ImportUsersDto, User>();
            CreateMap<ImportProductsDto, Product>();
            CreateMap<ImportCategoriesDto, Category>();
            CreateMap<ImportCategoriesProductsdto, CategoryProduct>();

            //export dtos
            CreateMap<Product, ExportProductsInRangeDto>()
                .ForMember(x=>x.BuyerFullName,mo=>mo.MapFrom(s=>$"{s.Buyer.FirstName} {s.Buyer.LastName}"));
        
        }
    }
}
