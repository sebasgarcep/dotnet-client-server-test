using Data;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class ApplicationFixture : IAsyncLifetime
{
    public WebApplication? WebApplication { get; private set; }
    public string Url { get => "http://localhost:5001"; }

    public async Task InitializeAsync()
    {
        DotEnv.Load(new DotEnvOptions(envFilePaths: [".env.test"]));

        var applicationBuilder = new ApplicationBuilder([]);
        applicationBuilder.AddDbContext();
        applicationBuilder.UseUrl(this.Url);

        this.WebApplication = applicationBuilder.GetWebApplication(atomic: false);

        using (var scope = this.WebApplication.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
        }

        await this.WebApplication.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (WebApplication != null)
        {
            await WebApplication.StopAsync();
            await WebApplication.DisposeAsync();
        }
    }
}