FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Shipping.Service.Testing/Shipping.Service.Testing.csproj
RUN dotnet publish Shipping.Service.Testing/Shipping.Service.Testing.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Shipping.Service.Testing.dll"]
