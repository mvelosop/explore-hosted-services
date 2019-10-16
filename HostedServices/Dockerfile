FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["HostedServices/HostedServices.csproj", "HostedServices/"]
RUN dotnet restore "HostedServices/HostedServices.csproj"
COPY . .
WORKDIR "/src/HostedServices"
RUN dotnet build "HostedServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HostedServices.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HostedServices.dll"]