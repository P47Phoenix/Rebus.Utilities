FROM microsoft/aspnetcore:2.0-nanoserver-1709 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0-nanoserver-1709 AS build
WORKDIR /src
COPY *.sln ./
COPY Rebus.ServiceHost.Api.Testing/Rebus.ServiceHost.Api.Testing.csproj Rebus.ServiceHost.Api.Testing/
RUN dotnet restore
COPY . .
WORKDIR /src/Rebus.ServiceHost.Api.Testing
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Rebus.ServiceHost.Api.Testing.dll"]
