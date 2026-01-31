using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using dotenv.net;

namespace Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            DotEnv.Load();

            var host = Environment.GetEnvironmentVariable("DATABASE_HOST") ?? "localhost";
            var database = Environment.GetEnvironmentVariable("DATABASE_NAME") ?? "testdb";
            var user = Environment.GetEnvironmentVariable("DATABASE_USER") ?? "testuser";
            var password = Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? "testpass";
            var port = Environment.GetEnvironmentVariable("DATABASE_PORT") ?? "15432";

            var connectionString = $"Host={host};Database={database};Username={user};Password={password};Port={port}";

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}