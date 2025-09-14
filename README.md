📦 Reto Técnico .NET - Sistema de Procesamiento de Productos



Este repositorio contiene la solución a un reto técnico que implementa un sistema distribuido para obtener, publicar y persistir productos utilizando una arquitectura de microservicios con .NET y Docker.



🏗️ Arquitectura de la Solución



El sistema está compuesto por cinco contenedores Docker orquestados con docker-compose:



AuthServer

API web que simula la autenticación y genera tokens JWT.



ProductsApi

API web que expone una lista de productos y está protegida, requiriendo un token JWT válido para el acceso.



ProductsPublisherAPI

Servicio orquestador. Llama a las dos APIs anteriores para obtener los datos y los publica en una cola de Azure Service Bus.



QueueWorker

Servicio en segundo plano que escucha la cola de Service Bus, procesa los mensajes y los persiste en la base de datos aplicando lógica de idempotencia.



sql-server-db

Contenedor con una instancia de SQL Server que funciona como la base de datos principal del sistema.



⚙️ Prerrequisitos



Antes de ejecutar la solución, asegúrate de tener instalado:



Docker



Docker Compose

&nbsp;(incluido en Docker Desktop)



🗄️ Configuración de la Base de Datos



El contenedor de SQL Server se levanta automáticamente con docker-compose.

Sin embargo, necesitas asegurarte de que la base de datos ProductsDB exista antes de que el Worker intente guardar productos.



🔹 Opciones:



Creación manual

Conéctate a la instancia de SQL Server (localhost:1433) y ejecuta:



CREATE DATABASE ProductsDB;





Creación automática con EF Core

El proyecto usa Entity Framework Core.

Puedes crear la base de datos y su esquema ejecutando migraciones:



dotnet ef database update --project QueueWorker





⚠️ Recomendación: Usa migraciones en lugar de EnsureCreated() para mantener el control de la evolución del esquema.



🔑 Configuración de Secretos



Es necesario crear archivos appsettings.Development.json con los secretos de conexión.



📌 ProductsPublisherAPI



Crear en: ProductsPublisherAPI/appsettings.Development.json



{

&nbsp; "ConnectionStrings": {

&nbsp;   "ServiceBus": "TU\_CONNECTION\_STRING\_DE\_AZURE\_SERVICE\_BUS"

&nbsp; }

}



📌 QueueWorker



Crear en: QueueWorker/appsettings.Development.json



{

&nbsp; "ConnectionStrings": {

&nbsp;   "ServiceBus": "TU\_CONNECTION\_STRING\_DE\_AZURE\_SERVICE\_BUS",

&nbsp;   "Database": "Server=localhost,1433;Database=ProductsDB;User=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;"

&nbsp; }

}





🔒 Nota: La contraseña (Password) debe coincidir con la SA\_PASSWORD definida en el archivo docker-compose.yml.



🚀 Cómo Levantar la Solución



Abre una terminal en la raíz del proyecto.



Ejecuta:



docker-compose up --build





La primera vez puede tardar unos minutos mientras se descargan las imágenes y se construyen los proyectos.



🧪 Cómo Realizar la Prueba Manual

1\. Iniciar el Proceso



Abre http://localhost:5301/swagger

.



Busca el endpoint POST /api/publish/products.



Haz clic en Try it out → Execute.



2\. Verificar los Mensajes



Sigue los logs del worker:



docker logs -f queue-worker





También puedes validar en el Portal de Azure que los mensajes llegan a la cola y luego desaparecen.



3\. Verificar la Base de Datos



Conéctate a la base de datos con SSMS o Azure Data Studio:



Servidor: localhost,1433



Usuario: sa



Contraseña: la configurada en docker-compose.yml.



Ejecuta:



SELECT \* FROM ProductsDB.dbo.Products;





Deberías ver los 5 productos insertados.



4\. Probar la Idempotencia



Ejecuta nuevamente el endpoint POST /api/publish/products.



En los logs del worker deberías ver mensajes como "Producto actualizado" en lugar de "insertado".



El número de filas en la tabla debe seguir siendo 5, con datos actualizados.



🛠️ Tecnologías Utilizadas



.NET 8



ASP.NET Core Web API



Entity Framework Core



Azure Service Bus



SQL Server



Docker \& Docker Compose



JWT Authentication

