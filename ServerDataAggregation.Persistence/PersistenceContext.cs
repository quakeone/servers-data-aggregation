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
        public DbSet<PlayerMatchProgress> PlayerMatchProgresses { get; set; }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var database = Environment.GetEnvironmentVariable("DB") ?? "servers";
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var user = Environment.GetEnvironmentVariable("DB_USER");
            var password = Environment.GetEnvironmentVariable("DB_PASS");
            var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5555";

            if (host == null || user == null || password == null)
            {
                throw new Exception("Database environment variables not set. Please set DB_HOST, DB_USER and DB_PASS");
            }
            // options.UseSqlite($"Data Source={DbPath}");
            options.UseNpgsql(
                $"Host={host};Database = {database}; Port = {port}; User id = {user}; Password = {password}",
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
