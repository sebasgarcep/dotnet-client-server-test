# Dotnet client-server test

- Find the correct way to init the db
- Find how to load env files properly as well
- Add controllers implementing auth and messages
- Add Kiota as a way to bridge client-server
- Add transactional automated tests

## Commands

```bash
# Create a migration
dotnet ef migrations add <Migration name> --project Projects/Server
# Run migrations
dotnet ef database update --project Projects/Server
```
