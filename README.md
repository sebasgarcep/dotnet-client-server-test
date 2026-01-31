# Dotnet client-server test

- Add transactional automated tests

## Commands

```bash
# Create a migration
dotnet ef migrations add <Migration name> --project Projects/Server
# Run migrations
dotnet ef database update --project Projects/Server
# Run dev server
dotnet watch run --project Projects/Server
```
