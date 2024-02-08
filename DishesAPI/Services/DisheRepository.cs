using AutoMapper;
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
        private readonly IMapper _mapper;

        public DisheRepository(DishesDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
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

        public void AddDishe(Dish dish)
        {
            var dishToReturn = _context.Dishes.Add(dish);
        }

        public void DeleteDishe(Dish dish)
        {
            _context.Dishes.Remove(dish);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DisheExistAsync(Guid dishId)
        {
            if(dishId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(dishId));
            }

            return await _context.Dishes.AnyAsync(d => d.Id == dishId);
        }
    }
}
