using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Entities;
using DishesAPI.Extensions;
using DishesAPI.Models;
using DishesAPI.ResourseParameters;
using DishesAPI.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MiniValidation;
using System.Net;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("BasicTokenAuthentication",
        new()
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Description = "provide a token to get access to all endpoint",
            In = ParameterLocation.Header
        });
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "BasicTokenAuthentication"
                }
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<DishesDbContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:DishesDbConnectionString"]);
});
builder.Services.AddScoped<IDishRepository, DisheRepository>();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("MustBeFromSudan", policy =>
    {
        policy.RequireRole("admin");
        policy.RequireClaim("country", "sudan");
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}


app.UseHttpsRedirection();

// Reigster all the endpoints

app.UseAuthentication();
app.UseAuthorization();

app.RigesterAllDishesEndpoint();
app.RigesterAllIngredientEndpoint();


//Make sure that the initial migration is excuted when run to seed the test data
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}


app.Run();
