FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY src/WorkoutLogger.Contracts/WorkoutLogger.Contracts.csproj src/WorkoutLogger.Contracts/
COPY src/WorkoutLogger.ServiceDefaults/WorkoutLogger.ServiceDefaults.csproj src/WorkoutLogger.ServiceDefaults/
COPY src/WorkoutLogger.API/WorkoutLogger.API.csproj src/WorkoutLogger.API/
RUN dotnet restore src/WorkoutLogger.API/WorkoutLogger.API.csproj

COPY src/WorkoutLogger.Contracts/ src/WorkoutLogger.Contracts/
COPY src/WorkoutLogger.ServiceDefaults/ src/WorkoutLogger.ServiceDefaults/
COPY src/WorkoutLogger.API/ src/WorkoutLogger.API/

RUN dotnet publish src/WorkoutLogger.API/WorkoutLogger.API.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "WorkoutLogger.API.dll"]
