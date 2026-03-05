FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Shipping.Api.Testing/Shipping.Api.Testing.csproj
RUN dotnet publish Shipping.Api.Testing/Shipping.Api.Testing.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Shipping.Api.Testing.dll"]
