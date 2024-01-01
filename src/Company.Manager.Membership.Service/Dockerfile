#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Company.iFX.Dapr/Company.iFX.Dapr.csproj", "src/Company.iFX.Dapr/"]
COPY ["src/Company.iFX.Hosting/Company.iFX.Hosting.csproj", "src/Company.iFX.Hosting/"]
COPY ["src/Company.Manager.Membership.Impl/Company.Manager.Membership.Impl.csproj", "src/Company.Manager.Membership.Impl/"]
COPY ["src/Company.Manager.Membership.Service/Company.Manager.Membership.Service.csproj", "src/Company.Manager.Membership.Service/"]
RUN dotnet restore "src/Company.Manager.Membership.Service/Company.Manager.Membership.Service.csproj"
COPY . .
WORKDIR "/src/src/Company.Manager.Membership.Service"
RUN dotnet build "Company.Manager.Membership.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Manager.Membership.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Manager.Membership.Service.dll"]