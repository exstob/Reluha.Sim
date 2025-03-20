using Sim.Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sim.Application.UseCases.SimulateLogicModel;

namespace Sim.Api;

public static class InitApplication
{
    public static void InitRepository(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SchemeDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();

            try
            {
                dbContext.Database.Migrate();
                logger.LogInformation("Database migrated successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError($"Database migration failed: {ex.Message}");
            }
        }
    }
}
