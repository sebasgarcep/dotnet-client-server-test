using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            DotEnv.Load();
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            var configuration = new Configuration();
            InitializeOptionsFromConfiguration(configuration, optionsBuilder);
            return new AppDbContext(optionsBuilder.Options);
        }

        public static void InitializeOptionsFromConfiguration(Configuration configuration, DbContextOptionsBuilder<AppDbContext> optionsBuilder)
        {
            var host = configuration.DatabaseHost;
            var database = configuration.DatabaseName;
            var user = configuration.DatabaseUser;
            var password = configuration.DatabasePassword;
            var port = configuration.DatabasePort;

            var connectionString = $"Host={host};Database={database};Username={user};Password={password};Port={port}";
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}