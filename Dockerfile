# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app /p:UseAppHost=false

# run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./
# Cloud Run injects PORT; listen on it
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
# (Optional) expose 8080 for local runs
EXPOSE 8080
ENTRYPOINT ["dotnet", "PresidentCountyAPI.dll"]
