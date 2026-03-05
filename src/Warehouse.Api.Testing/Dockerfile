FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore Warehouse.Api.Testing/Warehouse.Api.Testing.csproj
RUN dotnet publish Warehouse.Api.Testing/Warehouse.Api.Testing.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Warehouse.Api.Testing.dll"]
