FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Marketing.Service.Testing/Marketing.Service.Testing.csproj
RUN dotnet publish Marketing.Service.Testing/Marketing.Service.Testing.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Marketing.Service.Testing.dll"]
