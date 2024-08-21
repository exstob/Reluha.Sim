using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Sim.Domain.UiSchematic;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services
       .AddEndpointsApiExplorer()
       .AddSwaggerGen();

builder.Services.AddScoped<ICreateLogicModel, CreateLogicModel>();


var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the CORS middleware
app.UseCors("AllowSpecificOrigin");

app.MapPost("/simulate", (ICreateLogicModel creator, UiSchemeModel elements) =>
{
    var model = creator.Generate(elements);

    return Results.Ok(model.Id);
});


app.Run();

string Hello() 
{
    Console.WriteLine("Received");
    return "Hello World!";
}