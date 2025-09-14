# Etapa 1: Compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia el .sln y todos los .csproj para restaurar dependencias
COPY ["TechChallenge.sln", "."]
COPY ["AuthServer/AuthServer.csproj", "AuthServer/"]
COPY ["ProductsApi/ProductsApi.csproj", "ProductsApi/"]
COPY ["ProductsPublisherAPI/ProductsPublisherAPI.csproj", "ProductsPublisherAPI/"]
COPY ["QueueWorker/QueueWorker.csproj", "QueueWorker/"]
RUN dotnet restore "TechChallenge.sln"

# Copia el resto del código fuente
COPY . .
WORKDIR "/src/QueueWorker"
RUN dotnet build "QueueWorker.csproj" -c Release -o /app/build

# Etapa 2: Publicación
FROM build AS publish
RUN dotnet publish "QueueWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa 3: Ejecución (Usa la imagen 'runtime' que es más ligera, no 'aspnet')
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QueueWorker.dll"]