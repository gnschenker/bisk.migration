# run build from solution folder!
FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /app
COPY migration/migration.csproj ./migration/
COPY unittests/unittests.csproj ./unittests/
COPY unittests/packages packages
RUN dotnet restore --source /app/packages --source https://api.nuget.org/v3/index.json unittests/unittests.csproj
COPY migration migration
COPY unittests unittests
RUN dotnet build unittests/unittests.csproj
CMD dotnet test unittests/unittests.csproj