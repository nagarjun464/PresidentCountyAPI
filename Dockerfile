# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything (then discover the .csproj inside the image)
COPY . .

# Discover the first .csproj and publish with a stable assembly name
RUN set -eux; \
    CSProjPath="$(find . -name '*.csproj' -print -quit)"; \
    echo "Using project at: $CSProjPath"; \
    dotnet restore "$CSProjPath"; \
    dotnet publish "$CSProjPath" -c Release -o /app /p:UseAppHost=false /p:AssemblyName=app

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./

# Cloud Run sets PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 8080

ENTRYPOINT ["dotnet", "app.dll"]
