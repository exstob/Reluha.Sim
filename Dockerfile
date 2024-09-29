#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . ./
RUN dotnet restore "Reluha.Sim.sln"
RUN dotnet build "Reluha.Sim.sln" -c Release -o /app/build

# Build and run tests
WORKDIR /src/Sim.Tests
RUN dotnet build "Sim.Tests.csproj" -c Release
RUN dotnet test "Sim.Tests.csproj" -c Release --no-build --verbosity normal

FROM build AS publish
WORKDIR /src
RUN dotnet publish "Sim.Api/Sim.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sim.Api.dll"]