using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Sim.Domain.UiSchematic;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Application.UseCases;
using Sim.Application.UseCases.SimulateLogicModel;
using Sim.Application.Dtos.Simulate;
using Sim.Infrastructure;
using Sim.Application.UseCases.GetCircuitsUC;
using Sim.Application.UseCases.GetCircuitUC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
    config.AddAzureWebAppDiagnostics();
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://reluha.azurewebsites.net", "https://white-ocean-05ee55103.5.azurestaticapps.net")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services
       .AddEndpointsApiExplorer()
       .AddSwaggerGen()
       .AddMemoryCache();

builder.Services.AddSingleton<IRepository, Repository>();
builder.Services.AddTransient<ICreateLogicModel, CreateLogicModel>();
builder.Services.AddTransient<ISimulateLogicModel, SimulateLogicModel>();
builder.Services.AddTransient<IGetCircuitNamesUC, GetCircuitNamesUC>();
builder.Services.AddTransient<IGetCircuitUC, GetCircuitUC>();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Use the CORS middleware
app.UseCors("AllowSpecificOrigin");

var api_version = Environment.GetEnvironmentVariable("API__version");

app.MapPost($"/api/{api_version}/compile", async (ICreateLogicModel creator, UiSchemeModel elements) =>
{
    var initResult = await creator.Generate(elements).ConfigureAwait(false);
    Console.WriteLine(initResult.SchemeId);
    return Results.Ok(initResult);
});

app.MapPost($"/api/{api_version}/simulate", async (ISimulateLogicModel simulator, SimulateData simData) =>
{
    var result = await simulator.Simulate(simData).ConfigureAwait(false);
    return Results.Ok(result);
});

app.MapGet($"/api/{api_version}/circuits", (IGetCircuitNamesUC getCircuitNames) =>
{
    var result = getCircuitNames.GetCircuits();
    return Results.Ok(result);
});

app.MapGet($"/api/{api_version}/circuits/{{name}}", (IGetCircuitUC getCircuit, string name) =>
{
    var result = getCircuit.GetCircuit(name);
    return Results.Ok(result);
});


app.MapGet("/ping", () =>
{
    return Results.Ok($"API version is {api_version}");
});

app.MapGet("/", () =>
{
    return Results.Redirect("https://white-ocean-05ee55103.5.azurestaticapps.net");
});

app.Run();