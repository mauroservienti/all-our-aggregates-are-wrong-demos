FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Sales.Api.Testing/Sales.Api.Testing.csproj
RUN dotnet publish Sales.Api.Testing/Sales.Api.Testing.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sales.Api.Testing.dll"]
