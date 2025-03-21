# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY TBCPAY.GE.sln .
COPY API/API.csproj ./API/
COPY Application/Application.csproj ./Application/
COPY Domain/Domain.csproj ./Domain/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the code
COPY API/ ./API/
COPY Application/ ./Application/
COPY Domain/ ./Domain/
COPY Infrastructure/ ./Infrastructure/

# Build and publish the app
RUN dotnet publish API -c Release -o out

# Use the SDK image for the final container (for development)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS runtime
WORKDIR /app

# Copy the project files and source code for migrations
COPY --from=build /app/API/ ./API/
COPY --from=build /app/Application/ ./Application/
COPY --from=build /app/Domain/ ./Domain/
COPY --from=build /app/Infrastructure/ ./Infrastructure/
COPY --from=build /app/TBCPAY.GE.sln .

# Copy the published output to a subdirectory
COPY --from=build /app/out ./publish/

# Install EF Core tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# Expose the port your app runs on
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the app from the publish directory
ENTRYPOINT ["dotnet", "/app/publish/API.dll"]