# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# copy project + restore (layer caching)
COPY GymErp.csproj ./
RUN dotnet restore "GymErp.csproj"

# copy everything and publish
COPY . ./
RUN dotnet publish "GymErp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5035
ENTRYPOINT ["dotnet", "GymErp.dll"]

