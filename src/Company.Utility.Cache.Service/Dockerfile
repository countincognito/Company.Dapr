#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Company.iFX.Dapr/Company.iFX.Dapr.csproj", "src/Company.iFX.Dapr/"]
COPY ["src/Company.iFX.Hosting/Company.iFX.Hosting.csproj", "src/Company.iFX.Hosting/"]
COPY ["src/Company.Utility.Cache.Impl/Company.Utility.Cache.Impl.csproj", "src/Company.Utility.Cache.Impl/"]
COPY ["src/Company.Utility.Cache.Service/Company.Utility.Cache.Service.csproj", "src/Company.Utility.Cache.Service/"]
RUN dotnet restore "src/Company.Utility.Cache.Service/Company.Utility.Cache.Service.csproj"
COPY . .
WORKDIR "/src/src/Company.Utility.Cache.Service"
RUN dotnet build "Company.Utility.Cache.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Utility.Cache.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Utility.Cache.Service.dll"]