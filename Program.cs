using costumersApi.Data;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using costumersApi.Validators;
using costumersApi.Models;
using costumersApi.Middlewares;
using costumersApi.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CostumerValidator>();


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

app.MapGet("/costumers", async (AppDbContext context) =>
{
    var costumers = await context.Costumers.ToListAsync();
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

app.Run();
