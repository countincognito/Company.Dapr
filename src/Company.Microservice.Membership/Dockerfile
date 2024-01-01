#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Company.Access.User.Impl/Company.Access.User.Impl.csproj", "src/Company.Access.User.Impl/"]
COPY ["src/Company.Engine.Registration.Impl/Company.Engine.Registration.Impl.csproj", "src/Company.Engine.Registration.Impl/"]
COPY ["src/Company.iFX.Api/Company.iFX.Api.csproj", "src/Company.iFX.Api/"]
COPY ["src/Company.iFX.Hosting/Company.iFX.Hosting.csproj", "src/Company.iFX.Hosting/"]
COPY ["src/Company.iFX.Telemetry/Company.iFX.Telemetry.csproj", "src/Company.iFX.Telemetry/"]
COPY ["src/Company.Manager.Membership.Impl/Company.Manager.Membership.Impl.csproj", "src/Company.Manager.Membership.Impl/"]
COPY ["src/Company.Microservice.Membership.Data/Company.Microservice.Membership.Data.csproj", "src/Company.Microservice.Membership.Data/"]
COPY ["src/Company.Utility.Cache.Impl/Company.Utility.Cache.Impl.csproj", "src/Company.Utility.Cache.Impl/"]
COPY ["src/Company.Utility.Encryption.Impl/Company.Utility.Encryption.Impl.csproj", "src/Company.Utility.Encryption.Impl/"]
COPY ["src/Company.Microservice.Membership/Company.Microservice.Membership.csproj", "src/Company.Microservice.Membership/"]
RUN dotnet restore "src/Company.Microservice.Membership/Company.Microservice.Membership.csproj"
COPY . .
WORKDIR "/src/src/Company.Microservice.Membership"
RUN dotnet build "Company.Microservice.Membership.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Microservice.Membership.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Microservice.Membership.dll"]