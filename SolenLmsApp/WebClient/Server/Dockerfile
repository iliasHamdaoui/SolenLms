#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

COPY nginx/idp.crt /usr/local/share/ca-certificates/idp.crt
COPY nginx/api.crt /usr/local/share/ca-certificates/api.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SolenLmsApp/WebClient/Server/WebClient.Startup.csproj", "SolenLmsApp/WebClient/Server/"]
COPY ["SolenLmsApp/WebClient/Client/WebClient.csproj", "SolenLmsApp/WebClient/Client/"]
RUN dotnet restore "SolenLmsApp/WebClient/Server/WebClient.Startup.csproj"
COPY . .
WORKDIR "/src/SolenLmsApp/WebClient/Server"
RUN dotnet build "WebClient.Startup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebClient.Startup.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebClient.Startup.dll"]