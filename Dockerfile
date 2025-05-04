FROM node:18 AS npm-stage

WORKDIR /app

COPY UI/Web/package.json UI/Web/package-lock.json ./
RUN npm install

COPY UI/Web ./

RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY API/API.csproj API/
RUN dotnet restore "API/API.csproj"
COPY API API/
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /agora
COPY --from=publish /app/publish .
COPY --from=npm-stage /app/dist/web/browser ./wwwroot

ENTRYPOINT ["dotnet", "API.dll"]
