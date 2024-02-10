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

        dishesEndpoint.MapGet("", DishesEndpointHandlers.GetAllDishesEndpoint);
        dishesEndpoint.MapGet("/{disheId}", DishesEndpointHandlers.GetDishByIdEndpint).WithName("GetDishe");

        dishesEndpoint.MapPost("", DishesEndpointHandlers.AddDishEndpoint);
        dishWithGuidId.MapPut("", DishesEndpointHandlers.UpdateDishEndpoint)
            .AddEndpointFilter<DishLockedForUpdateAndDelete>();

        dishWithGuidId.MapDelete("", DishesEndpointHandlers.DeleteDishEndpoint)
            .AddEndpointFilter<DishLockedForUpdateAndDelete>();
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

