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

app.MapGet("dishes/{disheId:guid}", async Task<Results<NotFound, Ok<DisheDto>>> (IDishRepository repository,
    IMapper mapper,
    Guid disheId) =>
{
    var dishe = await repository.GetDishByIdAsync(disheId);
    if (dishe == null)
    {
        return TypedResults.NotFound();
    }
    

    return TypedResults.Ok(mapper.Map<DisheDto>(dishe));
});


app.MapGet("/dishes/{disheId}/ingredients", async (DishesDbContext context,
    IMapper mapper,
    Guid disheId) =>
{
    return mapper.Map<List<IngredientDto>>((await
        context.Dishes.Include(d => d.Ingredients).FirstOrDefaultAsync(d => d.Id == disheId))?.Ingredients);
});
//Make sure that the initial migration is excuted when run to seed the test data
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}


app.Run();
