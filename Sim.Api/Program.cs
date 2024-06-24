using Microsoft.AspNetCore.Builder;
using System.Text.Json;
using Sim.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Replace with your allowed origin(s)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services
       .AddEndpointsApiExplorer()
       .AddSwaggerGen();


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger()
//       .UseSwaggerUI();
//}

app.UseHttpsRedirection();

// Use the CORS middleware
app.UseCors("AllowSpecificOrigin");

app.MapPost("/simulate", async (SchemeElements elements) =>
{


    return Results.Ok("Hello");
});


app.Run();

string Hello() 
{
    Console.WriteLine("Received");
    return "Hello World!";
}