FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Primero copiamos todos los archivos para tener acceso al archivo de proyecto compatible
COPY . .

# Reemplazar el archivo de proyecto con la versión compatible con .NET 8.0
RUN if [ -f "TiengAnh.csproj.render" ]; then \
    cp TiengAnh.csproj.render TiengAnh.csproj && \
    echo "Usando proyecto compatible con .NET 8.0"; \
    fi

# Restaurar dependencias
RUN dotnet restore

# Compilar y publicar
RUN dotnet build --configuration Release --no-restore
RUN dotnet publish -c Release -o /app --no-build

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Make sure the json directory exists
RUN mkdir -p /app/json
# Make sure wwwroot directory exists
RUN mkdir -p /app/wwwroot

# Expose port
EXPOSE 8080

# Setup environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Run the application
ENTRYPOINT ["dotnet", "TiengAnh.dll"]
