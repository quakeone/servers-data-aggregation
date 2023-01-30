# Quake Servers Data Aggregation

## About 

This project powers the data collection backend of servers.quakeone.com

## Development Requirements

- dotnet 7.0 SDK
- ef core SDK
- postgres DB (compose file provided)

## Project setup

Copy the `.env.sample` from the root of the project as `.env' in your build output directory, and edit to suit your environment.

```
dotnet restore
dotnet build
```

