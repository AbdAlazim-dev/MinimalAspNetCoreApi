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
        var dishesEndpoint = app.MapGroup("/dishes")
            .RequireAuthorization()
            .WithOpenApi();
        var dishWithGuidId = dishesEndpoint.MapGroup("/{disheId:guid}");
        var dishWithGuidIdWithEndpointLockFilters = dishWithGuidId.MapGroup("")
            .AddEndpointFilter(new DishLockedForUpdateAndDelete(new List<Guid>()
            {
                //The guid of the dishes that cant be updated or deleted
                new Guid("eacc5169-b2a7-41ad-92c3-dbb1a5e7af06"),
                new Guid("98929bd4-f099-41eb-a994-f1918b724b5a")
            })).RequireAuthorization("MustBeFromSudan");

        dishesEndpoint.MapGet("", DishesEndpointHandlers.GetAllDishesEndpoint)
            .Produces<DisheDto>(StatusCodes.Status200OK)
            .WithSummary("Get all the dishes on the menu");
        dishesEndpoint.MapGet("/{disheId}", DishesEndpointHandlers.GetDishByIdEndpint)
            .ProducesProblem(StatusCodes.Status404NotFound).Produces<DisheDto>(StatusCodes.Status200OK)
            .WithName("GetDishe")
            .WithOpenApi()
            .WithSummary("Get one dish by Id")
            .WithDescription("Get dish by its uniqe identifier this identifier is GUID or uuid" +
            "if you passed GUID of unexisting dish you will get 404 not found");

        dishesEndpoint.MapPost("", DishesEndpointHandlers.AddDishEndpoint)
            .AddEndpointFilter<DishMustBeMoreThenThreeChar>()
            .Produces(StatusCodes.Status201Created)
            .WithSummary("Add a dish")
            .WithDescription("add dish to the list of dishes by providing" +
            "the dish name and it must be between 3-100 charctart");
        dishWithGuidIdWithEndpointLockFilters.MapPut("", DishesEndpointHandlers.UpdateDishEndpoint)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Update a dish")
            .WithDescription("Update a dish by providing a new value for the dish name" +
            "if you passed GUID of unexisting dish you will get 404 not found if its a valid Id" +
            "the dish will have the new name you passed in the request body"); ;

        dishWithGuidIdWithEndpointLockFilters.MapDelete("", DishesEndpointHandlers.DeleteDishEndpoint)
            .AddEndpointFilter<LogNotFoundResponseFilter>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status204NoContent)
            .WithSummary("Delete a dish by its ID")
            .WithDescription("delete dish by its uniqe identifier this identifier is GUID or uuid" +
            "if you passed GUID of unexisting dish you will get 404 not found"); ;
    }
    public static void RigesterAllIngredientEndpoint(this IEndpointRouteBuilder app)
    {
        var ingredientEndPoint = app.MapGroup("/dishes/{disheId:guid}/ingredients").RequireAuthorization();

        ingredientEndPoint.MapGet("", DishesEndpointHandlers.GetDishIngredientEndpoint)
            .Produces<IngredientDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithOpenApi(opreation =>
            {
                opreation.Summary = "get all the ingredients for the dish";
                opreation.Description = "Get all the dish ingreadients by providing " +
                "the dish identifier, if the dish id is not valid you will get 404";
                return opreation;
            });

    }
}

