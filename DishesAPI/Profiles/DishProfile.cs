using AutoMapper;
using DishesAPI.Models;

namespace DishesAPI.Profiles
{
    public class DishProfile : Profile
    {
        public DishProfile() 
        {
            CreateMap<Entities.Dish, Models.DisheDto>();
            CreateMap<Models.DisheDto, Entities.Dish>();
            CreateMap<DishForCreationDto, Entities.Dish>();
        }
    }
}
