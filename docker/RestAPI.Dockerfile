FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY Directory.Build.props Directory.Build.targets Directory.Packages.props global.json ./
COPY CleanDDDArchitecture.sln ./
COPY Hosts ./Hosts
COPY Domains ./Domains
COPY Library ./Library

RUN dotnet restore "Hosts/RestApi/Presentation/Presentation.csproj"
RUN dotnet publish "Hosts/RestApi/Presentation/Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
RUN apk add --no-cache icu-libs krb5-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanDDDArchitecture.Hosts.RestApi.Presentation.dll"]
