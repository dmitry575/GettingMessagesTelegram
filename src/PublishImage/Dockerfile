FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/PublishImage/PublishImage.csproj", "PublishImage/"]
RUN dotnet restore "src/PublishImage/PublishImage.csproj"
COPY . .
WORKDIR "/src/PublishImage"
RUN dotnet build "PublishImage.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PublishImage.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PublishImage.dll"]
