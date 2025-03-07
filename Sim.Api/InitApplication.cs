using Sim.Infrastructure.Models;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Sim.Api;

public static class InitApplication
{
    public static void InitRepository(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SchemeDbContext>();

            try
            {
                dbContext.Database.Migrate();
                Console.WriteLine("Database migrated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database migration failed: {ex.Message}");
            }
        }
    }
}
