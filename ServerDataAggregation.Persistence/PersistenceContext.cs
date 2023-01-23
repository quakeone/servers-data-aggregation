using Microsoft.EntityFrameworkCore;
using ServerDataAggregation.Persistence.Models;

namespace ServerDataAggregation.Persistence
{
    public class PersistenceContext : DbContext
    {
        public DbSet<Server> Servers { get; set; }
        public DbSet<ServerSnapshot> ServerSnapshots { get; set; }
        public DbSet<ServerState> ServerState { get; set; }
        public DbSet<ServerMatch> ServerMatches { get; set; }
        public DbSet<PlayerMatch> PlayerMatches { get; set; }

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //TODO: Possibly add something like BaseEntityConfiguration?
            modelBuilder.ApplyConfiguration(new ServerSnapshotConfiguration());
            modelBuilder.ApplyConfiguration(new ServerStateConfiguration());
        }
    }
}
