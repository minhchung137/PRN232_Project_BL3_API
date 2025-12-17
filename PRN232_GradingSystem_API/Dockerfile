# Multi-stage build for PRN232_GradingSystem_API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy entire solution (đơn giản, dễ bảo trì)
COPY . .

# Restore & publish API project
WORKDIR /src/PRN232_Project_BL3_API/PRN232_GradingSystem_API/PRN232_GradingSystem_API
RUN dotnet restore \
    && dotnet publish -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Chạy trên port 8080 (phù hợp Render/Docker)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "PRN232_GradingSystem_API.dll"]


