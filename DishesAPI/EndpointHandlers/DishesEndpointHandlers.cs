using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Models;
using DishesAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Security.Claims;

namespace DishesAPI.EndpointHandlers
{
    public class DishesEndpointHandlers
    {

        public async static Task<Ok<IEnumerable<DisheDto>>> GetAllDishesEndpoint(
        IDishRepository repository,
        ClaimsPrincipal principal,
        IMapper mapper)
        {
            Console.WriteLine($"is this user authinticated ? {principal.Identity?.IsAuthenticated};" +
                $" and his name is {principal.FindFirst("name")?.Value}");
            return TypedResults.Ok(mapper.Map<IEnumerable<DisheDto>>(await repository.GetAllAsync()));
        }
        public async static Task<Results<NotFound, Ok<DisheDto>>> GetDishByIdEndpint(IDishRepository repository,
        IMapper mapper,
        Guid disheId)
        {
            var dishe = await repository.GetDishByIdAsync(disheId);
            if (dishe == null)
            {
                return TypedResults.NotFound();
            }


            return TypedResults.Ok(mapper.Map<DisheDto>(dishe));
        }
        public async static Task<Ok<List<IngredientDto>>> GetDishIngredientEndpoint(DishesDbContext context,
        IMapper mapper,
        Guid disheId)
        {
            return TypedResults.Ok(mapper.Map<List<IngredientDto>>((await
                context.Dishes.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == disheId))?.Ingredients));
        }
        public async static Task<CreatedAtRoute<DisheDto>> AddDishEndpoint(IDishRepository dishRepository,
        IMapper mapper,
        DishForCreationDto dish)
        {

            var dishEntity = mapper.Map<Dish>(dish);

            dishRepository.AddDishe(dishEntity);

            await dishRepository.SaveChangesAsync();


            var dishToReturn = mapper.Map<DisheDto>(dishEntity);

            return TypedResults.CreatedAtRoute(
                    dishToReturn,
                    "GetDishe",
                    new
                    {
                        disheId = dishToReturn.Id
                    });
        }
        public static async Task<Results<NotFound, NoContent>> UpdateDishEndpoint(
        IDishRepository dishRepository,
        IMapper mapper,
        Guid disheId,
        DishForCreationDto dish)
        {
            var dishEntity = await dishRepository.GetDishByIdAsync(disheId);

            if (dishEntity == null)
            {
                TypedResults.NotFound("There is no dish with the id : {dishId}");
            }
            if (MiniValidator.TryValidate(dish, out var error))
            {
                TypedResults.ValidationProblem(error);
            }
            mapper.Map(dish, dishEntity);

            await dishRepository.SaveChangesAsync();


            return TypedResults.NoContent();
        }
        public static async Task<Results<NotFound, NoContent>> DeleteDishEndpoint
        (IDishRepository repository,
            Guid disheId,
            ClaimsPrincipal claimsPrinciple
            )
        {
            Console.WriteLine($"this user is from :{claimsPrinciple.FindFirst("country")?.Value} " +
                $"and his is an : {claimsPrinciple.IsInRole}");
            var dishEntity = await repository.GetDishByIdAsync(disheId);
            if (dishEntity == null)
            {
                return TypedResults.NotFound();
            }
            repository.DeleteDishe(dishEntity);

            await repository.SaveChangesAsync();
            return TypedResults.NoContent();
        }
    }
}
