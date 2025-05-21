FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore --disable-parallel

# Copy all files and build with detailed logging
COPY . .
RUN dotnet build --configuration Release --no-restore || (echo "Build failed" && cat /tmp/build.log && exit 1)
RUN dotnet publish -c Release -o /app --no-build || (echo "Publish failed" && exit 1)

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
