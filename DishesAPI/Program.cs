using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Models;
using DishesAPI.ResourseParameters;
using DishesAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MiniValidation;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DishesDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:DishesDbConnectionString"]);
});
builder.Services.AddScoped<IDishRepository, DisheRepository>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/dishes", async  Task<Ok<IEnumerable<DisheDto>>>(
    IDishRepository repository,
    IMapper mapper) =>
{

    return TypedResults.Ok(mapper.Map<IEnumerable<DisheDto>>(await repository.GetAllAsync()));
});

app.MapGet("dishes/{disheId}", async Task<Results<NotFound, Ok<DisheDto>>> (IDishRepository repository,
    IMapper mapper,
    Guid disheId) =>
{
    var dishe = await repository.GetDishByIdAsync(disheId);
    if (dishe == null)
    {
        return TypedResults.NotFound();
    }
    

    return TypedResults.Ok(mapper.Map<DisheDto>(dishe));
}).WithName("GetDishe");


app.MapGet("/dishes/{disheId}/ingredients", async (DishesDbContext context,
    IMapper mapper,
    Guid disheId) =>
{
    return mapper.Map<List<IngredientDto>>((await
        context.Dishes.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == disheId))?.Ingredients);
});
// manipulating the dishes resourses 


// Add dishe route
app.MapPost("/dishes", async (IDishRepository dishRepository,
    IMapper mapper,
    DishForCreationDto dish) =>
{
    // For some reson this code dose not work im using miniValidator to check if the data is valid
    //if (MiniValidator.TryValidate(dish, out var error))
    //{
    //    return Results.ValidationProblem(error);
    //}

    var dishEntity = mapper.Map<Dish>(dish);

    dishRepository.AddDishe(dishEntity);

    if(!await dishRepository.SaveChangesAsync())
    {
        return Results.Problem("Error while Saving the Dish",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    var dishToReturn = mapper.Map<DisheDto>(dishEntity);

    return Results.CreatedAtRoute( "GetDishe", new { disheId = dishEntity.Id}, dishToReturn);
});

//Update a Dishe
app.MapPut("/dishes/{dishId}", async (
    IDishRepository dishRepository,
    IMapper mapper,
    Guid dishId,
    DishForCreationDto dish) =>
{
    var dishEntity = await dishRepository.GetDishByIdAsync(dishId);

    if (dishEntity == null)
    {
        Results.Problem("There is no dish with the id : {dishId}",
            statusCode: StatusCodes.Status404NotFound);
    }
    if(MiniValidator.TryValidate(dish, out var error))
    {
        Results.ValidationProblem(error);
    }
    mapper.Map(dish, dishEntity);

    if(!await dishRepository.SaveChangesAsync())
    {
        return Results.Problem("Error acoure while trying to update the dish",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return Results.Ok(mapper.Map<DisheDto>(dishEntity));
});
//Delete a Dishe
app.MapDelete("/dishes/{dishId}", async (
    IDishRepository dishRerpository,
    Guid dishId) =>
{
    // get the dishe we want to delete 
    var dishEnitity = await dishRerpository.GetDishByIdAsync(dishId);

    if (dishEnitity == null)
    {
        return Results.Problem($"There is no dishe with the id {dishId}",
            statusCode: StatusCodes.Status404NotFound);
    }

    dishRerpository.DeleteDishe(dishEnitity);

    if (!await dishRerpository.SaveChangesAsync())
    {
        return Results.Problem("Error while deleting the dish",
            statusCode: StatusCodes.Status500InternalServerError);
    }

    return Results.NoContent();
});



//Make sure that the initial migration is excuted when run to seed the test data
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}


app.Run();
