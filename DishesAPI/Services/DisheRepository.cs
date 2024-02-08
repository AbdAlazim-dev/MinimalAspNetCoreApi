using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Models;
using DishesAPI.ResourseParameters;
using Microsoft.EntityFrameworkCore;
using System;

namespace DishesAPI.Services
{
    public class DisheRepository : IDishRepository
    {
        private readonly DishesDbContext _context;

        public DisheRepository(DishesDbContext context) 
        {
            _context = context;
        }
        public async Task<IEnumerable<Entities.Dish>> GetAllAsync()
        {
            return await _context.Dishes.ToListAsync();

        }

        public Task<PagedList<Dish>> GetAllAsync(DisheParamters parameter)
        {
            if(parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            var collection = _context.Dishes as IQueryable<Dish>;

            if (!string.IsNullOrEmpty(parameter.Name))
            {
                var name = parameter.Name.Trim();
                collection = collection.Where(d => d.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(parameter.SearchQuery))
            {
                var searchQuery = parameter.SearchQuery.Trim();
                collection = collection.Where((d) => d.Name.Contains(searchQuery));
            }
            return PagedList<Dish>.CreateAsync(collection, parameter.PageNumber, parameter.PageSize);
        }

        public async Task<Dish> GetDishByIdAsync(Guid dishId)
        {
            return await _context.Dishes.Where(d => d.Id == dishId).FirstOrDefaultAsync();
        }
        //public async Task<Dish> GitDishIngredient(Guid dishId)
        //{
        //    var ingredient = await _context.Dishes.Include(d => d.Ingredients)
        //        .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients);
        //}
    }
}
