#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

RUN apt-get update && apt-get install -y ffmpeg

COPY nginx/idp.crt /usr/local/share/ca-certificates/idp.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SolenLmsApp/Api/Startup/WebApi/WebApi.Startup.csproj", "SolenLmsApp/Api/Startup/WebApi/"]
COPY ["SolenLmsApp/Api/Shared/Infrastructure/Shared.Infrastructure.csproj", "SolenLmsApp/Api/Shared/Infrastructure/"]
COPY ["SolenLmsApp/Api/CourseManagement/Src/CourseManagement.csproj", "SolenLmsApp/Api/CourseManagement/Src/"]
COPY ["SolenLmsApp/Api/Shared/Core/Shared.Core.csproj", "SolenLmsApp/Api/Shared/Core/"]
COPY ["SolenLmsApp/Api/Shared/Presentation/WebApi/Shared.WebApi.csproj", "SolenLmsApp/Api/Shared/Presentation/WebApi/"]
COPY ["SolenLmsApp/Api/Learning/Src/Learning.csproj", "SolenLmsApp/Api/Learning/Src/"]
COPY ["SolenLmsApp/Api/Resources/Src/Resources.csproj", "SolenLmsApp/Api/Resources/Src/"]
RUN dotnet restore "SolenLmsApp/Api/Startup/WebApi/WebApi.Startup.csproj"
COPY . .
WORKDIR "/src/SolenLmsApp/Api/Startup/WebApi"
RUN dotnet build "WebApi.Startup.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi.Startup.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.Startup.dll"]