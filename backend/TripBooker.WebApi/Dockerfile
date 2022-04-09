FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

# Copy needed projects
COPY TripBooker.Common TripBooker.Common
COPY TripBooker.WebApi TripBooker.WebApi

# Build project
WORKDIR /src/TripBooker.WebApi
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
COPY --from=build-env /src/TripBooker.WebApi/out .
ENTRYPOINT ["dotnet", "TripBooker.WebApi.dll"]
