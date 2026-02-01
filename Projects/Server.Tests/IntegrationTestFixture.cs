using Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

[Collection("ApplicationCollection")]
public class IntegrationTestFixture : IAsyncLifetime
{
    private readonly ApplicationFixture ApplicationFixture;
    protected HttpClient Client;

    private IDbContextTransaction? DbContextTransaction;

    public IntegrationTestFixture(ApplicationFixture applicationFixture)
    {
        this.ApplicationFixture = applicationFixture;
        this.Client = new HttpClient { BaseAddress = new Uri(applicationFixture.Url) };
    }

    public async Task InitializeAsync()
    {
        var dbContext = this.ApplicationFixture.WebApplication!.Services.GetRequiredService<AppDbContext>();
        this.DbContextTransaction = await dbContext.Database.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        if (this.DbContextTransaction != null)
        {
            await this.DbContextTransaction.RollbackAsync();
            await this.DbContextTransaction.DisposeAsync();
        }
    }
}
