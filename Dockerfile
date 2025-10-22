# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Publish the project
RUN dotnet publish "StringAnalyzer.API/StringAnalyzer.API.csproj" -c Release -o /app/out

# Use the runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Expose port 8080 (Railway default)
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "StringAnalyzer.API.dll"]
