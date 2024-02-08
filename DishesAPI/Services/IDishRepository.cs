using DishesAPI.Models;
using DishesAPI.ResourseParameters;

namespace DishesAPI.Services
{
    public interface IDishRepository
    {
        public Task<IEnumerable<Entities.Dish>> GetAllAsync();
        public Task<PagedList<Entities.Dish>> GetAllAsync(DisheParamters parameter);
        public Task<Entities.Dish> GetDishByIdAsync(Guid dishId);

    }
}
