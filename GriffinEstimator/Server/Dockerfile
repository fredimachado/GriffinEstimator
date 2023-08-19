#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GriffinEstimator/Server/GriffinEstimator.Server.csproj", "GriffinEstimator/Server/"]
COPY ["GriffinEstimator/Client/GriffinEstimator.Client.csproj", "GriffinEstimator/Client/"]
COPY ["GriffinEstimator/Shared/GriffinEstimator.Shared.csproj", "GriffinEstimator/Shared/"]
RUN dotnet restore "GriffinEstimator/Server/GriffinEstimator.Server.csproj"
COPY . .
WORKDIR "/src/GriffinEstimator/Server"
RUN dotnet build "GriffinEstimator.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GriffinEstimator.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GriffinEstimator.Server.dll"]
