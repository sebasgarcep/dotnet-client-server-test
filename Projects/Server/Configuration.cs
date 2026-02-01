public class Configuration
{
    public string DatabaseName { get => Environment.GetEnvironmentVariable("DATABASE_NAME") ?? ""; }
    public string DatabaseHost { get => Environment.GetEnvironmentVariable("DATABASE_HOST") ?? ""; }
    public string DatabasePort { get => Environment.GetEnvironmentVariable("DATABASE_PORT") ?? ""; }
    public string DatabaseUser { get => Environment.GetEnvironmentVariable("DATABASE_USER") ?? ""; }
    public string DatabasePassword { get => Environment.GetEnvironmentVariable("DATABASE_PASSWORD") ?? ""; }
    public string JwtSecret { get => Environment.GetEnvironmentVariable("JWT_SECRET") ?? ""; }
}