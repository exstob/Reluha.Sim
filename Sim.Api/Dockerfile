#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Sim.Api/Sim.Api.csproj", "Sim.Api/"]
RUN dotnet restore "Sim.Api/Sim.Api.csproj"
COPY . .
WORKDIR "/src/Sim.Api"
RUN dotnet build "Sim.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sim.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sim.Api.dll"]