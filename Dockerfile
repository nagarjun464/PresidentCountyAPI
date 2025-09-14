# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .

# Discover the .csproj and publish with a stable assembly name
RUN set -eux; \
    CSProjPath="$(find . -name '*.csproj' -print -quit)"; \
    echo "Using project at: $CSProjPath"; \
    dotnet restore "$CSProjPath"; \
    dotnet publish "$CSProjPath" -c Release -o /app /p:UseAppHost=false /p:AssemblyName=app

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app ./

# Cloud Run provides PORT 
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "app.dll"]
