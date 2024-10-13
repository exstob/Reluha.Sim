using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Sim.Domain.UiSchematic;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Application.UseCases;
using Sim.Application.UseCases.SimulateLogicModel;
using Sim.Application.Dtos.Simulate;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://reluha.azurewebsites.net")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services
       .AddEndpointsApiExplorer()
       .AddSwaggerGen()
       .AddMemoryCache();

builder.Services.AddTransient<ICreateLogicModel, CreateLogicModel>();
builder.Services.AddTransient<ISimulateLogicModel, SimulateLogicModel>();


var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use the CORS middleware
app.UseCors("AllowSpecificOrigin");

app.MapPost("/compile", async (ICreateLogicModel creator, UiSchemeModel elements) =>
{
    var initResult = await creator.Generate(elements);
    Console.WriteLine(initResult.SchemeId);
    return Results.Ok(initResult);
});

app.MapPost("/simulate", async (ISimulateLogicModel simulator, SimulateData simData) =>
{
    var result = await simulator.Simulate(simData);
    return Results.Ok(result);
});


app.MapGet("/", () =>
{
    return Results.Ok("Hello!!!");
});

app.MapGet("/front", () =>
{
    Results.Redirect("https://white-ocean-05ee55103.5.azurestaticapps.net");
});

Console.Write("Before run app");

app.Run();