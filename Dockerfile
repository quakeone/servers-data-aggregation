FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

COPY . .
RUN dotnet restore

# copy and publish app and libraries
RUN dotnet publish -c Release -o /app --no-restore

COPY lib/* /app

FROM mcr.microsoft.com/dotnet/runtime:7.0

WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "ServersDataAggregation.App.dll"]
