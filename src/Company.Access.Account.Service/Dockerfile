#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Company.Access.Account.Impl/Company.Access.Account.Impl.csproj", "src/Company.Access.Account.Impl/"]
COPY ["src/Company.iFX.Nats/Company.iFX.Nats.csproj", "src/Company.iFX.Nats/"]
COPY ["src/Company.iFX.Hosting/Company.iFX.Hosting.csproj", "src/Company.iFX.Hosting/"]
COPY ["src/Company.Access.Account.Service/Company.Access.Account.Service.csproj", "src/Company.Access.Account.Service/"]
RUN dotnet restore "src/Company.Access.Account.Service/Company.Access.Account.Service.csproj"
COPY . .
WORKDIR "/src/src/Company.Access.Account.Service"
RUN dotnet build "Company.Access.Account.Service.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Access.Account.Service.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Access.Account.Service.dll"]