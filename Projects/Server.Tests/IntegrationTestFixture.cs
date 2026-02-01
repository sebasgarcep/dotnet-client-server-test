using Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

[Collection("ApplicationCollection")]
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
public class IntegrationTestFixture : IAsyncLifetime
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
{
    private readonly ApplicationFixture ApplicationFixture;
    protected HttpClient Client { get; private set; }

    private IDbContextTransaction? DbContextTransaction;

    public IntegrationTestFixture(ApplicationFixture applicationFixture)
    {
        this.ApplicationFixture = applicationFixture;
        this.Client = new HttpClient { BaseAddress = new Uri(ApplicationFixture.Url) };
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

        this.Client.Dispose();
    }
}
