# Steg 1: Bygg applikationen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
# Kopiera csproj och återställ beroenden
COPY . ./
# Restore as distinct layers
RUN dotnet restore

# Kopiera övriga filer och bygg applikationen
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Exponera porten som applikationen körs på
EXPOSE 8080

# Starta applikationen
ENTRYPOINT ["dotnet", "microservice-delete.dll"]