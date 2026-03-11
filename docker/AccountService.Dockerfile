FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Directory.Build.props Directory.Build.targets Directory.Packages.props global.json ./
COPY CleanDDDArchitecture.sln ./
COPY Hosts ./Hosts
COPY Domains ./Domains
COPY Library ./Library

RUN dotnet restore "Hosts/Services/AccountService/Presentation/Presentation.csproj"
RUN dotnet publish "Hosts/Services/AccountService/Presentation/Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 wget \
    && rm -rf /var/lib/apt/lists/*
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanDDDArchitecture.Hosts.Services.AccountService.Presentation.dll"]
