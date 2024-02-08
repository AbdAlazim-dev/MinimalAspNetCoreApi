using AutoMapper;

namespace DishesAPI.Profiles
{
    public class DishProfile : Profile
    {
        public DishProfile() 
        {
            CreateMap<Entities.Dish, Models.DisheDto>();
            
        }
    }
}
