FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY Directory.Build.props Directory.Build.targets Directory.Packages.props global.json ./
COPY CleanDDDArchitecture.sln ./
COPY Hosts ./Hosts
COPY Domains ./Domains
COPY Library ./Library

RUN dotnet restore "Hosts/Services/TodoService/Presentation/Presentation.csproj"
RUN dotnet publish "Hosts/Services/TodoService/Presentation/Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CleanDDDArchitecture.Hosts.Services.TodoService.Presentation.dll"]
