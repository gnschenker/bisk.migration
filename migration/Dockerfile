FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /app
COPY migration.csproj ./
COPY packages packages
RUN dotnet restore --source /app/packages --source https://api.nuget.org/v3/index.json
COPY . .
CMD dotnet run