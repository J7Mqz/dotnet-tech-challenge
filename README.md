Reto Técnico .NET - Sistema de Procesamiento de Productos
Este repositorio contiene la solución a un reto técnico que implementa un sistema distribuido simple para obtener, publicar y persistir productos utilizando una arquitectura de microservicios con .NET y Docker.

Arquitectura de la Solución
El sistema está compuesto por cinco contenedores Docker que orquesta docker-compose:

AuthServer: Una API web responsable de simular la autenticación y generar tokens JWT.

ProductsApi: Una API web que expone una lista de productos y está protegida, requiriendo un token JWT válido para el acceso.

ProductsPublisherAPI: El servicio orquestador. Llama a las otras dos APIs para obtener los datos y los publica en una cola de Azure Service Bus.

QueueWorker: Un servicio de fondo que escucha la cola de Service Bus, procesa los mensajes y persiste la información en la base de datos, implementando lógica de idempotencia.

sql-server-db: Un contenedor con una instancia de SQL Server que sirve como la base de datos real para el sistema.

Prerrequisitos
Para levantar y ejecutar esta solución, solo necesitas tener instalado:

Docker

Docker Compose (usualmente viene incluido con Docker Desktop)

Cómo Levantar la Solución
Sigue estos pasos para poner en marcha todo el sistema:

Clonar el Repositorio

Shell

git clone <URL_DE_TU_REPOSITORIO>
cd <NOMBRE_DEL_REPOSITORIO>
Configurar Secretos
Es necesario crear dos archivos de configuración para los secretos.

Para el Publisher: Crea un archivo appsettings.Development.json dentro de la carpeta ProductsPublisherAPI/.

JSON

{
  "ConnectionStrings": {
    "ServiceBus": "TU_CONNECTION_STRING_DE_AZURE_SERVICE_BUS"
  }
}
Para el Worker: Crea un archivo appsettings.Development.json dentro de la carpeta QueueWorker/.

JSON

{
  "ConnectionStrings": {
    "ServiceBus": "TU_CONNECTION_STRING_DE_AZURE_SERVICE_BUS",
    "Database": "Server=localhost,1433;Database=ProductsDB;User=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"
  }
}
Nota: La contraseña de la base de datos (Password) debe coincidir con la SA_PASSWORD definida en el archivo docker-compose.yml.

Levantar los Contenedores
Abre una terminal en la raíz del proyecto y ejecuta el siguiente comando. La primera vez puede tardar unos minutos mientras se descargan las imágenes y se construyen los proyectos.

Shell

docker-compose up --build
Cómo Realizar la Prueba Manual
Una vez que todos los contenedores estén corriendo, sigue este flujo para probar el sistema:

Iniciar el Proceso:

Abre tu navegador y ve a la interfaz de Swagger de la API de publicación: http://localhost:5301/swagger.

Busca el endpoint POST /api/publish/products, haz clic en "Try it out" y luego en "Execute".

Verificar los Mensajes:

Puedes ver los logs del worker en tiempo real para confirmar que está recibiendo y procesando los mensajes con el comando:

Shell

docker logs -f queue-worker
También puedes verificar que los mensajes llegaron (y luego desaparecieron) de tu cola en el Portal de Azure.

Verificar la Base de Datos:

Conéctate a la base de datos usando tu herramienta preferida (SSMS, Azure Data Studio, etc.).

Servidor: localhost,1433

Usuario: sa

Contraseña: La que configuraste en docker-compose.yml.

Ejecuta una consulta para ver los datos insertados:

SQL

SELECT * FROM ProductsDB.dbo.Products;
Deberías ver los 5 productos insertados.

Probar la Idempotencia:

Vuelve a ejecutar el endpoint POST /api/publish/products desde Swagger.

Revisa los logs del worker: ahora deberían mostrar mensajes de "Producto actualizado" en lugar de "insertado".

Vuelve a consultar la base de datos: el número de filas no debe haber cambiado (seguirán siendo 5), solo sus datos se habrán actualizado.

Tecnologías Utilizadas
.NET 8

ASP.NET Core Web API

Entity Framework Core

Azure Service Bus

SQL Server

Docker & Docker Compose

Autenticación con JWT