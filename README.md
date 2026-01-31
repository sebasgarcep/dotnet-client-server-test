# Dotnet client-server test

- Add created at/updated at timestamps to models
- Add Kiota as a way to bridge client-server
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
