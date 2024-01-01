#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Company.Engine.Registration.Impl/Company.Engine.Registration.Impl.csproj", "src/Company.Engine.Registration.Impl/"]
COPY ["src/Company.iFX.Dapr/Company.iFX.Dapr.csproj", "src/Company.iFX.Dapr/"]
COPY ["src/Company.iFX.Hosting/Company.iFX.Hosting.csproj", "src/Company.iFX.Hosting/"]
COPY ["src/Company.Engine.Registration.Service/Company.Engine.Registration.Service.csproj", "src/Company.Engine.Registration.Service/"]
RUN dotnet restore "src/Company.Engine.Registration.Service/Company.Engine.Registration.Service.csproj"
COPY . .
WORKDIR "/src/src/Company.Engine.Registration.Service"
RUN dotnet build "Company.Engine.Registration.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Engine.Registration.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Engine.Registration.Service.dll"]