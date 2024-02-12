using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.EndpointFilters;
using DishesAPI.EndpointHandlers;
using DishesAPI.Entities;
using DishesAPI.Models;
using DishesAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using System.Runtime.CompilerServices;

namespace DishesAPI.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RigesterAllDishesEndpoint(this IEndpointRouteBuilder app)
    {
        var dishesEndpoint = app.MapGroup("/dishes");
        var dishWithGuidId = dishesEndpoint.MapGroup("/{disheId:guid}");
        var dishWithGuidIdWithEndpointLockFilters = dishWithGuidId.MapGroup("")
            .AddEndpointFilter(new DishLockedForUpdateAndDelete(new List<Guid>()
            {
                //The guid of the dishes that cant be updated or deleted
                new Guid("eacc5169-b2a7-41ad-92c3-dbb1a5e7af06"),
                new Guid("98929bd4-f099-41eb-a994-f1918b724b5a")
            }));

        dishesEndpoint.MapGet("", DishesEndpointHandlers.GetAllDishesEndpoint);
        dishesEndpoint.MapGet("/{disheId}", DishesEndpointHandlers.GetDishByIdEndpint).WithName("GetDishe");

        dishesEndpoint.MapPost("", DishesEndpointHandlers.AddDishEndpoint)
            .AddEndpointFilter<DishMustBeMoreThenThreeChar>();
        dishWithGuidIdWithEndpointLockFilters.MapPut("", DishesEndpointHandlers.UpdateDishEndpoint);
            



        dishWithGuidIdWithEndpointLockFilters.MapDelete("", DishesEndpointHandlers.DeleteDishEndpoint)
            .AddEndpointFilter<LogNotFoundResponseFilter>();
    }
    public static void RigesterAllIngredientEndpoint(this IEndpointRouteBuilder app)
    {
        var ingredientEndPoint = app.MapGroup("/dishes/{disheId:guid}/ingredients");

        ingredientEndPoint.MapGet("", DishesEndpointHandlers.GetDishIngredientEndpoint);

        ingredientEndPoint.MapPost("", () =>
        {
            throw new NotImplementedException();
        });

    }
}

