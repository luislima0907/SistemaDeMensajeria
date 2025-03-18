# Programación III - 2025
## Grupo No.1
### Integrantes
- **Luis Carlos Lima Pérez**
- **Angelica María Mejía Tzoc**
- **Mynor Ebenezer Alonso Miranda**
- **Josué Emanuel Ramírez Aquino**
- **Josseline Emerita Galeano Hernández**

# Sistema de Mensajería con RabbitMQ

Este proyecto demuestra un sistema de mensajería utilizando RabbitMQ con tres componentes: una aplicación web ASP.NET Core, una aplicación de consola y una configuración con Docker para ejecutar los servicios.

## Estructura del Proyecto

El sistema consta de dos aplicaciones principales:

1. **SistemaDeMensajeriaWeb**: Aplicación web ASP.NET Core
2. **SistemaDeMensajeriaConsola**: Aplicación de consola para enviar mensajes

## SistemaDeMensajeriaConsola

Aplicación de consola que permite a los usuarios enviar mensajes a una cola de RabbitMQ.

### Características
- Conexión a servidor RabbitMQ local o en contenedor
- Interfaz de línea de comandos interactiva
- Envío de mensajes a una cola de RabbitMQ persistente

### Tecnologías
- .NET 9.0
- RabbitMQ.Client 7.1.1
- Async/await para operaciones asíncronas

### Archivos principales
- **Program.cs**: Punto de entrada de la aplicación y contiene la lógica principal para conectarse a RabbitMQ y enviar mensajes a la cola. Además Implementa un bucle interactivo para capturar mensajes de usuario.

### Instalación de paquetes
- En caso de no tener instalado el paquete de RabbitMQ.Client, se puede instalar con el siguiente comando en la terminal con esta ruta:
    ```
    cd ./SistemaDeMensajeria/SistemaDeMensajeriaConsola
    ```
    ```bash
    dotnet add package RabbitMQ.Client --version 7.1.1
    ```

### Uso
1. Ejecute la aplicación ubicandose en la carpeta del proyecto de consola:
    ```
    cd ./SistemaDeMensajeria/SistemaDeMensajeriaConsola
    ```
    ```bash
    dotnet run
    ```
2. Escriba un mensaje en la línea de comandos
3. El mensaje se enviará a la cola "mensajes_queue"
4. Escriba "salir" para terminar de ejecutar la aplicación, o bien, puede dejarlo vacio y presionar enter para terminar.

## SistemaDeMensajeriaWeb

Aplicación web ASP.NET Core que se integra con RabbitMQ.

### Características
- Configuración para ejecución en Docker
- Integración con RabbitMQ mediante contenedores Docker

### Tecnologías
- ASP.NET Core (.NET 9.0)
- RabbitMQ.Client 7.1.1
- Docker y Docker Compose

### Archivos principales
- **Dockerfile:** Define la imagen de Docker para la aplicación web, utilizando .NET SDK para compilar y .NET Runtime para ejecutar la aplicación.
- **Program.cs:** Punto de entrada de la aplicación y configuración de los servicios para el contenedor de Docker, además define la ruta en donde aparecerán los mensajes obtenidos de la cola.
- **RabbitMQConsumerService.cs:** Servicio que consume mensajes de la cola de RabbitMQ "mensajes_queue" y los almacena en una cola para mostrarlos en la interfaz web.

### Instalacion de paquetes
- En caso de no tener instalado el paquete de RabbitMQ.Client, se puede instalar con el siguiente comando en la terminal con esta ruta:
    ```
    cd ./SistemaDeMensajeria/SistemaDeMensajeriaWeb
    ```
    ```bash
    dotnet add package RabbitMQ.Client --version 7.1.1
    ```

### Uso local
1. Ejecute la aplicación ubicandose en la carpeta del proyecto de web:
    ```
    cd ./SistemaDeMensajeria/SistemaDeMensajeriaWeb
    ```
    ```bash
    dotnet run
    ```
2. Visualice los mensajes enviados desde la linea de comandos.

## Configuración de Docker

El proyecto incluye configuración de Docker para facilitar la implementación:

### Docker Compose (docker-compose.yml)
- **RabbitMQ:** Servidor de mensajería con interfaz de administración
  - Puertos: 5672 (AMQP), 15672 (Management UI)
  - Credenciales predefinidas
  - Persistencia de datos mediante volúmenes

- **Web:** Aplicación web ASP.NET Core
  - Puerto: 5000 (externo) -> 8080 (interno)
  - Depende de RabbitMQ
  - Variables de entorno configuradas para conexión a RabbitMQ

### Ejecución con Docker
1. Para construir las imágenes de Docker ejecute este comando en la ruta raíz del proyecto: 
    ```
    cd ./SistemaDeMensajeria
    ```
    ```bash
    docker-compose build
    ```
2. Luego para iniciar los servicios de colas RabbitMQ y Web para el contenedor principal, ejecute este comando:
    ```bash
    docker-compose up -d
    ```
3. Si quiere ver los logs en el contenedor de Docker puede ejecutar el siquiente comando:
    ```bash
    docker-compose logs -f
    ```
4. Finalmente, para detener los servicios del contenedor de Docker ejecute el siguiente comando:
    ```bash
    docker-compose down
    ```
O bien para detenerlos forzosamente:
```
Ctrl + c
```

### Acceso a la interfaz web de RabbitMQ
Una vez que los contenedores estén en ejecución, puede acceder a la interfaz de administración de RabbitMQ:
- **URL:** `http://localhost:15672`
- **Usuario:** systemesgg
- **Contraseña:** messag3312j$

### Acceso a la interfaz web que recibe los mensajes que se envían por medio de la consola
Una vez que los contenedores estén en ejecución, puede acceder a la interfaz web para visualizar los mensajes recibidos:
- **URL:** `http://localhost:5000:8080`

## Requisitos previos para utilizar el sistema de mensajería
- **.NET 9.0 SDK**
- **Docker y Docker Compose (para ejecución containerizada)**
- **IDE recomendado:** Visual Studio 2022 o JetBrains Rider

### Comandos útiles para el desarrollo
- **Restaurar las dependencias del proyecto de consola o web:**
    ```bash
    dotnet restore
    ```
- **Compilar el proyecto de consola o web:**
    ```bash
    dotnet build
    ```
- **Ejecutar el proyecto en consola o web de manera local sin Docker:**
    ```bash
    dotnet run
    ```

## Link para ver las diapositivas y el diagrama de arquitectura
- https://www.canva.com/design/DAGiC0DGgSY/NoBuY5AdlxJ1uv0e7Ye6dw/edit?utm_content=DAGiC0DGgSY&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton
