# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["TankR.csproj", "./"]
RUN dotnet restore "TankR.csproj"

# Copy everything else and publish
COPY . .
RUN dotnet publish "TankR.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TankR.dll"]
