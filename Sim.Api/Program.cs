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

//builder.Services.AddHostedMqttServer(options =>
//{
//    options.WithDefaultEndpoint();
//    options.WithDefaultEndpointPort(1883);
//    options.WithConnectionBacklog(100);

//});    

// Add WebSocket and TCP server adapters
//builder.Services.AddMqttTcpServerAdapter();
//builder.Services.AddMqttWebSocketServerAdapter();

//builder.Services.AddMqttConnectionHandler();
builder.Services.AddConnections();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowSpecificOriginPolicy");

var api_version = Environment.GetEnvironmentVariable("API__version");

app.MapHub<SimulateHub>("/simulate-hub");

app.MapPost($"/api/{api_version}/build", async (IBuildLogicModel builder, UiSchemeModel elements) =>
{
    var buildResult = await builder.Generate(elements).ConfigureAwait(false);
    Console.WriteLine(buildResult.SchemeId);
    return Results.Ok(buildResult);
});

app.MapPost($"/api/{api_version}/run", async (IRunLogicModel creator, UiSchemeModel elements) =>
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


app.MapGet("/ping", async (MqClient client) =>
{
    await client.Ping_Server();
    return Results.Ok($"API version is {api_version}");
});

app.MapGet("/", () =>
{
    return Results.Redirect("https://white-ocean-05ee55103.5.azurestaticapps.net");
});

// Configure MQTT application
app.UseWebSockets();
//app.UseMqttServer(server =>
//{
//    var mqttServices = app.Services.GetRequiredService<MqBroker>();
//    mqttServices.InitMqttServer(server);

//    server.StartedAsync += async args =>
//    {
//        var logger = app.Services.GetRequiredService<ILogger<Program>>();
//        logger.LogInformation("MQTT Server started.");
//        await Task.CompletedTask;
//    };

//    server.ClientConnectedAsync += mqttServices.OnClientConnected;
//    server.InterceptingPublishAsync += mqttServices.OnIntercepted;

//});

app.Run();