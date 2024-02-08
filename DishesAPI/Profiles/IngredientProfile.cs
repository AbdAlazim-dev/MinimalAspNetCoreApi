using AutoMapper;

namespace DishesAPI.Profiles
{
    public class IngredientProfile : Profile
    {
        public IngredientProfile() 
        {
            CreateMap<Entities.Ingredient, Models.IngredientDto>()
                .ForMember(d => d.DishId, 
                o => o.MapFrom(s => s.Dishes.First().Id));
        }
    }
}
