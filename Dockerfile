# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything (we'll locate the csproj inside the image)
COPY . .

# Discover the first .csproj (assumes one project)
# If you have more than one, replace this with the correct relative path.
RUN set -eux; \
    CSProjPath="$(find . -name '*.csproj' -print -quit)"; \
    echo "Using project at: $CSProjPath"; \
    dotnet restore "$CSProjPath"; \
    dotnet publish "$CSProjPath" -c Release -o /app /p:UseAppHost=false /p:AssemblyName=app

# ---------- runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./

# Cloud Run sets PORT; listen on it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 8080
ENTRYPOINT ["dotnet", "PresidentCountyAPI.dll"]
