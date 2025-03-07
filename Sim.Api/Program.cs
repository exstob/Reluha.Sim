using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Sim.Domain.UiSchematic;
using Sim.Application.UseCases.CreateLogicModel;
using Sim.Application.UseCases;
using Sim.Application.UseCases.SimulateLogicModel;
using Sim.Application.UseCases.SendSimulateState;
using Sim.Application.MqttServices;
using Sim.Application.Dtos.Simulate;
using Sim.Infrastructure;
using Sim.Application.UseCases.GetCircuitsUC;
using Sim.Application.UseCases.GetCircuitUC;
using Sim.Application.UseCases.BuildLogicModel;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.PortableExecutable;
using MQTTnet.AspNetCore;
using MQTTnet.Server;
using Microsoft.Extensions.Options;
using MQTTnet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sim.Infrastructure.Models;
using System;
using Sim.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();
    config.AddDebug();
    config.AddAzureWebAppDiagnostics();
});

builder.Services.AddDbContext<SchemeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOriginPolicy",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://reluha.azurewebsites.net", "https://white-ocean-05ee55103.5.azurestaticapps.net")
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .AllowAnyHeader();
        });
});

builder.Services
       .AddEndpointsApiExplorer()
       .AddSwaggerGen()
       .AddMemoryCache();

//builder.Services.ConfigureHttpJsonOptions(options =>
//{
//    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
//});

builder.Services.AddSignalR();

builder.Services.AddSingleton<IRepository, Repository>();
builder.Services.AddTransient<IRunLogicModel, RunLogicModel>();
builder.Services.AddTransient<IBuildLogicModel, BuildLogicModel > ();
builder.Services.AddTransient<ISimulateLogicModel, SimulateLogicModel>();
builder.Services.AddTransient<IGetCircuitNamesUC, GetCircuitNamesUC>();
builder.Services.AddTransient<IGetCircuitUC, GetCircuitUC>();
builder.Services.AddSingleton<SimulateHub>();

builder.Services.InjectMqttServices();

builder.Services.AddConnections();


var webApp = builder.Build();

if (webApp.Environment.IsDevelopment())
{
    webApp.UseSwagger()
       .UseSwaggerUI();
}

webApp.InitRepository();
webApp.UseHttpsRedirection();
webApp.UseStaticFiles();

webApp.UseRouting();

webApp.UseCors("AllowSpecificOriginPolicy");

var api_version = Environment.GetEnvironmentVariable("API__version");

webApp.MapHub<SimulateHub>("/simulate-hub");

webApp.MapPost($"/api/{api_version}/build", async (IBuildLogicModel builder, UiSchemeModel elements) =>
{
    var buildResult = await builder.Generate(elements).ConfigureAwait(false);
    Console.WriteLine(buildResult.SchemeId);
    return Results.Ok(buildResult);
});

webApp.MapPost($"/api/{api_version}/run", async (IRunLogicModel creator, UiSchemeModel elements) =>
{
    var initResult = await creator.Generate(elements).ConfigureAwait(false);
    Console.WriteLine(initResult.SchemeId);
    return Results.Ok(initResult);
});

webApp.MapPost($"/api/{api_version}/simulate", async (ISimulateLogicModel simulator, SimulateData simData) =>
{
    var result = await simulator.Simulate(simData).ConfigureAwait(false);
    return Results.Ok(result);
});

webApp.MapGet($"/api/{api_version}/circuits", (IGetCircuitNamesUC getCircuitNames) =>
{
    var result = getCircuitNames.GetCircuits();
    return Results.Ok(result);
});

webApp.MapGet($"/api/{api_version}/circuits/{{name}}", (IGetCircuitUC getCircuit, string name) =>
{
    var result = getCircuit.GetCircuit(name);
    return Results.Ok(result);
});


webApp.MapGet("/ping", () =>
{
    return Results.Ok($"API version is {api_version}");
});

webApp.MapGet("/", () =>
{
    return Results.Redirect("https://white-ocean-05ee55103.5.azurestaticapps.net");
});

// Configure MQTT application
webApp.UseWebSockets();


webApp.InitMqttServices();

webApp.Run();