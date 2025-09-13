# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy solution (if you have one) and the project file
# COPY *.sln .                    # uncomment if you have a .sln
COPY PresidentCountyAPI/*.csproj PresidentCountyAPI/
RUN dotnet restore PresidentCountyAPI/PresidentCountyAPI.csproj

# copy the rest and publish
COPY . .
RUN dotnet publish PresidentCountyAPI/PresidentCountyAPI.csproj -c Release -o /app /p:UseAppHost=false

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app ./

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 8080
ENTRYPOINT ["dotnet", "PresidentCountyAPI.dll"]
