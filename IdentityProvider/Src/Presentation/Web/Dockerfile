#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["IdentityProvider/Src/Presentation/Web/IdentityProvider.Startup.csproj", "IdentityProvider/Src/Presentation/Web/"]
COPY ["IdentityProvider/Src/Infrastructure/IdentityProvider.Infrastructure.csproj", "IdentityProvider/Src/Infrastructure/"]
COPY ["IdentityProvider/Src/Core/IdentityProvider.Core.csproj", "IdentityProvider/Src/Core/"]
COPY ["SolenLmsApp/Api/Shared/Core/Shared.Core.csproj", "SolenLmsApp/Api/Shared/Core/"]
COPY ["IdentityProvider/Src/Presentation/WebApi/IdentityProvider.WebApi.csproj", "IdentityProvider/Src/Presentation/WebApi/"]
RUN dotnet restore "IdentityProvider/Src/Presentation/Web/IdentityProvider.Startup.csproj"
COPY . .
WORKDIR "/src/IdentityProvider/Src/Presentation/Web"
RUN dotnet build "IdentityProvider.Startup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityProvider.Startup.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityProvider.Startup.dll"]