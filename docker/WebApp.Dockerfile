FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app

# Install cultures
RUN apk add --no-cache icu-libs

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
# Currently this copies the whole solution (including submodules)
# in a smaller solution with small number of projects it would be best
# to copy only the relevant projects with COPY ["<project>/<project>.csproj", "<project>"]
COPY . ./
RUN dotnet restore "Hosts/WebApp/Presentation/Presentation.csproj"
COPY . ./
WORKDIR /src/Hosts/WebApp/Presentation
RUN dotnet build --no-restore "Presentation.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "Presentation.csproj" -c Release -o /app/publish

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanDDDArchitecture.Hosts.WebApp.Presentation.dll"]
