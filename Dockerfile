FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-stable \
      --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DockerizedPuppeteerSharp.csproj", "."]
RUN dotnet restore "./DockerizedPuppeteerSharp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "DockerizedPuppeteerSharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerizedPuppeteerSharp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerizedPuppeteerSharp.dll"]