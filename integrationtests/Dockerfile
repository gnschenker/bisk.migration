FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /app
COPY integrationtests.csproj ./
COPY packages packages
RUN dotnet restore --source /app/packages --source https://api.nuget.org/v3/index.json integrationtests.csproj
COPY . .
RUN dotnet build
CMD dotnet test