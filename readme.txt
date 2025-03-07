Add migration
dotnet ef migrations add Initial  --project Sim.Infrastructure --startup-project Sim.Api

Push migration
dotnet ef database update --project Sim.Infrastructure --startup-project Sim.Api
