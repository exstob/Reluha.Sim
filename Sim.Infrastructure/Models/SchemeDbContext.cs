using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Infrastructure.Models;

public class SchemeDbContext: DbContext
{
    public SchemeDbContext(DbContextOptions<SchemeDbContext> options) : base(options)
    {
    }

    public DbSet<Scheme> Schemes { get; set; }
}




public class Scheme
{
    public int Id { get; set; }
    public required string Name { get; set; }
    [Column(TypeName = "jsonb")]
    public required Dictionary<string, object> Payload { get; set; }
}
