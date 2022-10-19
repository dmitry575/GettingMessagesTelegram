FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GettingMessagesTelegram/GettingMessagesTelegram.csproj", "GettingMessagesTelegram/"]
RUN dotnet restore "GettingMessagesTelegram/GettingMessagesTelegram.csproj"
COPY . .
WORKDIR "/src/GettingMessagesTelegram"
RUN dotnet build "GettingMessagesTelegram.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GettingMessagesTelegram.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GettingMessagesTelegram.dll"]
