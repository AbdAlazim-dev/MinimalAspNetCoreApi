using DishesAPI.Entities;
using DishesAPI.Models;
using DishesAPI.ResourseParameters;

namespace DishesAPI.Services
{
    public interface IDishRepository
    {
        public Task<IEnumerable<Entities.Dish>> GetAllAsync();
        public Task<PagedList<Entities.Dish>> GetAllAsync(DisheParamters parameter);
        public Task<Entities.Dish> GetDishByIdAsync(Guid dishId);
        public void AddDishe(Dish dish);
        public void DeleteDishe(Dish dish);
        Task<bool> SaveChangesAsync();
        Task<bool> DisheExistAsync(Guid dishId);

    }
}
