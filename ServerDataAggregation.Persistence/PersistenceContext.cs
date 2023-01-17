using Microsoft.EntityFrameworkCore;
using ServerDataAggregation.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerDataAggregation.Persistence
{
    public class PersistenceContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerStatus> ServerStatuses { get; set; }
        public DbSet<ServerSnapshot> ServerSnapshots { get; set; }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // options.UseSqlite($"Data Source={DbPath}");
            options.UseNpgsql(
                $"Host=localhost; Database = servers; Port = 5555; User id = docker; Password = docker",
                x => x.MigrationsAssembly("ServerDataAggregation.Persistence")
            );
        }
    }
}
