using costumersApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using costumersApi.Validators;
using costumersApi.Models;
using costumersApi.Middlewares;
using costumersApi.Exceptions;
using costumersApi.Infrastructure.Cache;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Runtime.InteropServices.Marshalling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CostumerValidator>();

builder.Services.AddScoped<ICachingService, CachingService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis:6379"; //endereço porta Redis
    options.InstanceName = "instance"; // Prefixo para as chaves do Redis
});


var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapPost("/costumers", async (AppDbContext context, IValidator<Costumer> validator, Costumer costumer) =>
{
    var validationResult = await validator.ValidateAsync(costumer);
    if (!validationResult.IsValid)
    {
        throw new FluentValidation.ValidationException(validationResult.Errors);
    }

    context.Costumers.Add(costumer);
    await context.SaveChangesAsync();
    return Results.Created($"/costumers/{costumer.Id}", costumer);
});

app.MapGet("/costumers", async (AppDbContext context, ICachingService cache) =>
{
    string cacheKey = "allCostumers";
    var costumersCached = await cache.GetAsync(cacheKey);

    List<Costumer>? costumers;

    if (!string.IsNullOrEmpty(costumersCached))
    {
        costumers = JsonSerializer.Deserialize<List<Costumer>>(costumersCached);
        return Results.Ok(costumers);
    }

    costumers = await context.Costumers.ToListAsync();
    await cache.SetAsync(cacheKey, JsonSerializer.Serialize(costumers));
    return Results.Ok(costumers);
});

app.MapDelete("/costumers/{id}", async (AppDbContext context, int id) =>
{
    var costumer = await context.Costumers.FindAsync(id);
    if (costumer == null)
    {
        throw new CostumerNotFoundException(id);
    }

    context.Costumers.Remove(costumer);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/costumers/{id}", async (AppDbContext context, int id, ICachingService cache) =>
{
    var costumerCache = await cache.GetAsync(id.ToString());
    Costumer? costumer;

    if (!string.IsNullOrWhiteSpace(costumerCache))
    {
        costumer = JsonSerializer.Deserialize<Costumer>(costumerCache);
        return Results.Ok(costumer);
    }

    costumer = await context.Costumers.FindAsync(id);

    if (costumer == null)
    {
        throw new CostumerNotFoundException(id);
    }

    await cache.SetAsync(id.ToString(), JsonSerializer.Serialize<Costumer>(costumer));

    return Results.Ok(costumer);
});

app.Run();
