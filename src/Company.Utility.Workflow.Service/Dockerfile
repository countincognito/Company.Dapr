#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Company.Utility.Workflow.Service/Company.Utility.Workflow.Service.csproj", "src/Company.Utility.Workflow.Service/"]
RUN dotnet restore "src/Company.Utility.Workflow.Service/Company.Utility.Workflow.Service.csproj"
COPY . .
WORKDIR "/src/src/Company.Utility.Workflow.Service"
RUN dotnet build "Company.Utility.Workflow.Service.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Company.Utility.Workflow.Service.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Company.Utility.Workflow.Service.dll"]