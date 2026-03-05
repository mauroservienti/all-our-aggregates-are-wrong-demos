FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Warehouse.Service.Testing/Warehouse.Service.Testing.csproj
RUN dotnet publish Warehouse.Service.Testing/Warehouse.Service.Testing.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Warehouse.Service.Testing.dll"]
