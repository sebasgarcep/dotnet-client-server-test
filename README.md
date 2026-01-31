# Dotnet client-server test

## Commands

```bash
# Create a migration
dotnet ef migrations add <Migration name> --project Projects/Server
# Run migrations
dotnet ef database update --project Projects/Server
```