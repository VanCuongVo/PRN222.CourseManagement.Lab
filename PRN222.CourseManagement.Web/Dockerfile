# See https://aka.ms/customizecontainer to learn how to customize your debug container

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# ---------- BUILD ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["PRN222.CourseManagement.Web/PRN222.CourseManagement.Web.csproj", "PRN222.CourseManagement.Web/"]
COPY ["PRN222.CourseManagement.Repository/PRN222.CourseManagement.Repository.csproj", "PRN222.CourseManagement.Repository/"]
COPY ["PRN222.CourseManagement.ServiceLayer/PRN222.CourseManagement.ServiceLayer.csproj", "PRN222.CourseManagement.ServiceLayer/"]

RUN dotnet restore "./PRN222.CourseManagement.Web/PRN222.CourseManagement.Web.csproj"

COPY . .

WORKDIR "/src/PRN222.CourseManagement.Web"

RUN dotnet build "PRN222.CourseManagement.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build


# ---------- PUBLISH ----------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/PRN222.CourseManagement.Web

RUN dotnet publish "PRN222.CourseManagement.Web.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# ---------- FINAL ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "PRN222.CourseManagement.Web.dll"]