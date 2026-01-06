# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar
COPY ["PlataformaCursos.Api/PlataformaCursos.Api.csproj", "PlataformaCursos.Api/"]
COPY ["PlataformaCursos.Application/PlataformaCursos.Application.csproj", "PlataformaCursos.Application/"]
COPY ["PlataformaCursos.Domain/PlataformaCursos.Domain.csproj", "PlataformaCursos.Domain/"]
COPY ["PlataformaCursos.Infrastructure/PlataformaCursos.Infrastructure.csproj", "PlataformaCursos.Infrastructure/"]

RUN dotnet restore "PlataformaCursos.Api/PlataformaCursos.Api.csproj"

# Copiar todo el código y publicar
COPY . .
WORKDIR "/src/PlataformaCursos.Api"
RUN dotnet publish "PlataformaCursos.Api.csproj" -c Release -o /app/publish

# Etapa 2: Ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Puerto por defecto de los contenedores .NET 8
ENV ASPNETCORE_HTTP_PORTS=80
ENTRYPOINT ["dotnet", "PlataformaCursos.Api.dll"]