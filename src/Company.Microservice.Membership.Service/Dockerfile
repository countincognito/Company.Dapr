#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Company.iFX.Api/Company.iFX.Api.csproj", "src/Company.iFX.Api/"]
COPY ["src/Company.iFX.Dapr/Company.iFX.Dapr.csproj", "src/Company.iFX.Dapr/"]
COPY ["src/Company.iFX.Hosting/Company.iFX.Hosting.csproj", "src/Company.iFX.Hosting/"]
COPY ["src/Company.iFX.Proxy/Company.iFX.Proxy.csproj", "src/Company.iFX.Proxy/"]
COPY ["src/Company.Manager.Membership.Interface/Company.Manager.Membership.Interface.csproj", "src/Company.Manager.Membership.Interface/"]
COPY ["src/Company.Microservice.Membership.Data/Company.Microservice.Membership.Data.csproj", "src/Company.Microservice.Membership.Data/"]
COPY ["src/Company.Microservice.Membership.Service/Company.Microservice.Membership.Service.csproj", "src/Company.Microservice.Membership.Service/"]
RUN dotnet restore "src/Company.Microservice.Membership.Service/Company.Microservice.Membership.Service.csproj"
COPY . .
WORKDIR "/src/src/Company.Microservice.Membership.Service"
RUN dotnet build "Company.Microservice.Membership.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Microservice.Membership.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Microservice.Membership.Service.dll"]