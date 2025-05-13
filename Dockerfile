FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY src/YnabMcpServer/YnabMcpServer.csproj ./src/YnabMcpServer/
RUN dotnet restore ./src/YnabMcpServer/YnabMcpServer.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish src/YnabMcpServer/YnabMcpServer.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000
ENTRYPOINT ["dotnet", "YnabMcpServer.dll"]
