# Instruções



## Solution

* dotnet --version 10.0.102

## 

## Requisitos

* .NET 8 SDK instalado.
* IDE como Visual Studio, VS Code, Rider ou outra de sua preferência.

## 

## Executando a Aplicação

1. Navegue até a pasta do projeto API:

&#x20;  bash
   cd src/ApiParfois/Desafio.API
   

2. Execute a aplicação:

&#x20;  bash
   dotnet run
   

3. Acesse o Swagger para explorar os endpoints:

&#x20;  http://localhost:5032/swagger/index.html

## 

## Testes

1. Navegue até o projeto de testes:

&#x20;  bash
   cd ../Desafio.API
   

2. Execute os testes:

&#x20;  bash
   dotnet test   

## 

## Estrutura do Projeto

* **Domain**: Entidades e regras de dominio.
* **Application**: Services, Modelos, regra de negocio.
* **Infrastructure**: Repositórios EF InMemory.
* **API**: Controllers, DI, Middleware.
* **Testes**: Unitários e integração.

ApiParfois/

&#x20;       ├── ApiParfois.slnx

&#x20;       │

&#x20;       ├── Desafio.API/

&#x20;       │   ├── Controllers/

&#x20;       │   │   ├── PedidoController.cs

&#x20;       │   │   └── StatusController.cs

&#x20;       │   ├── Middleware/

&#x20;       │   │   └── GlobalExceptionHandler.cs

&#x20;       │   ├── Properties/

&#x20;       │   │   └── launchSettings.json

&#x20;       │   └── Program.cs

&#x20;       │

&#x20;       ├── Desafio.Application/

&#x20;       │   ├── Models/

&#x20;       │   │   ├── PedidoRequest.cs

&#x20;       │   │   ├── PedidoResponse.cs

&#x20;       │   │   ├── StatusRequest.cs

&#x20;       │   │   └── StatusResponse.cs

&#x20;       │   ├── Services/

&#x20;       │   │   ├── IPedidoService.cs

&#x20;       │   │   ├── IStatusService.cs

&#x20;       │   │   ├── PedidoService.cs

&#x20;       │   │   └── StatusService.cs

&#x20;       │   └── Validators/

&#x20;       │       ├── IStatusValidator.cs

&#x20;       │       └── StatusValidator.cs

&#x20;       │

&#x20;       ├── Desafio.Domain/

&#x20;       │   ├── Entities/

&#x20;       │   │   ├── Entity.cs

&#x20;       │   │   ├── ItemPedido.cs

&#x20;       │   │   └── Pedido.cs

&#x20;       │   ├── Enums/

&#x20;       │   │   └── StatusPedido.cs

&#x20;       │   └── Interfaces/

&#x20;       │       └── IPedidoRepository.cs

&#x20;       │

&#x20;       ├── Desafio.Infrastructure/

&#x20;       │   ├── Data/

&#x20;       │   │   ├── AppDbContext.cs

&#x20;       │   │   └── Configurations/

&#x20;       │   │       ├── ItemPedidoConfiguration.cs

&#x20;       │   │       └── PedidoConfiguration.cs

&#x20;       │   └── Repositories/

&#x20;       │       └── PedidoRepository.cs

&#x20;       │

&#x20;       └── Desafio.Testes/

&#x20;           ├── Integration/

&#x20;           │   └── Controllers/

&#x20;           │       ├── PedidoControllerTests.cs

&#x20;           │       └── StatusControllerTests.cs

&#x20;           │

&#x20;           └── Unit/

&#x20;               ├── Application/

&#x20;               │   └── Validators/

&#x20;               │       └── StatusValidatorTests.cs

&#x20;               ├── Domain/

&#x20;               │   ├── ItemPedidoTests.cs

&#x20;               │   └── PedidoTests.cs

&#x20;               └── Services/

&#x20;                   ├── PedidoServiceTests.cs

&#x20;                   └── StatusServiceTests.cs

## 

