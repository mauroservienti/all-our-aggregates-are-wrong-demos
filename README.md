# All our Aggregates are Wrong - Demos

A microservices-powered e-commerce shopping cart sample based on SOA principles. These are the demos for my [All our Aggregates are Wrong](https://milestone.topics.it/talks/all-our-aggregates-are-wrong.html) talk.

The demo showcases a shopping cart and its associated behaviors. Add items to the cart and observe the log messages printed in the various service terminal windows as events are processed. Leave the cart inactive for a few seconds and observe the stale cart policy kick in: after 30 seconds of inactivity a `ShoppingCartGotStale` event is published, and after 60 seconds the cart is deleted and a `ShoppingCartGotInactive` event is published.

## Solution overview

The solution is organized into four business domains, each owning its own data and communicating asynchronously via NServiceBus over RabbitMQ.

### Marketing

Manages the product catalog. The `Marketing.Api` exposes product data (names, descriptions) used to compose product listing and shopping cart views. `Marketing.Data` provides the EF Core data access layer backed by a dedicated PostgreSQL database.

### Sales

Manages the shopping cart lifecycle. `Sales.Api` exposes endpoints to retrieve cart contents. `Sales.Service` is a background NServiceBus endpoint that handles `ProductAddedToCart` commands and runs the `ShoppingCartLifecyclePolicy` saga, which publishes `ShoppingCartGotStale` (after 30 seconds of inactivity) and `ShoppingCartGotInactive` (after 60 seconds of inactivity, also deleting the cart). `Sales.Data` provides the EF Core data access layer backed by a dedicated PostgreSQL database.

### Shipping

Handles shipping concerns when items are added to a cart. `Shipping.Api` exposes shipping-related cart data. `Shipping.Service` is a background NServiceBus endpoint that reacts to cart events. `Shipping.Data` provides the EF Core data access layer backed by a dedicated PostgreSQL database.

### Warehouse

Manages inventory. `Warehouse.Api` exposes stock availability data. `Warehouse.Service` is a background NServiceBus endpoint that reacts to cart events to track product reservations. `Warehouse.Data` provides the EF Core data access layer backed by a dedicated PostgreSQL database.

### WebApp

An ASP.NET Core web application that serves as the user interface. It composes views by coordinating responses from all service APIs using the ServiceComposer pattern (via `ITOps.ViewModelComposition`). It hosts a product catalog page and a shopping cart page.

### Shared and infrastructure projects

- **`ITOps.ViewModelComposition`** – ServiceComposer infrastructure for assembling composite view models from multiple services.
- **`ITOps.Middlewares`** – ASP.NET Core middleware shared across services (e.g., shopping cart resolution).
- **`NServiceBus.Shared`** – Common NServiceBus endpoint configuration (transport, persistence, metrics).
- **`JsonUtils`** – JSON serialization utilities shared across the solution.
- **`CreateRequiredDatabases`** – A utility project that creates and seeds the PostgreSQL databases required by all services.
- **`*.Messages` / `*.Messages.Events`** – NServiceBus message contracts (commands and events) for each domain.
- **`*.ViewModelComposition` / `*.ViewModelComposition.Events` / `*.ViewModelComposition.Messages`** – ServiceComposer handlers and events used to contribute data from each service to the composite view models.

## Requirements

The following requirements must be met to run the demos successfully:

- [Visual Studio Code](https://code.visualstudio.com/) and the [Dev containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers).
- [Docker](https://www.docker.com/get-started) must be pre-installed on the machine.
- The repository `devcontainer` setup requires `docker-compose` to be installed on the machine.

## How to configure Visual Studio Code to run the demos

- Clone the repository
  - On Windows, make sure to clone on a short path, e.g., `c:\dev`, to avoid any "path too long" error
- Open the repository root folder in Visual Studio Code
- Make sure Docker is running
  - If you're using Docker for Windows with Hyper-V, make sure that the cloned folder, or a parent folder, is mapped in Docker
- Open the Visual Studio Code command palette (`F1` on all supported operating systems, for more information on VS Code keyboard shortcuts, refer to [this page](https://www.arungudelli.com/microsoft/visual-studio-code-keyboard-shortcut-cheat-sheet-windows-mac-linux/))
- Type `Reopen in Container`, the command palette supports auto-completion; the command should be available by typing `reop`

Wait for Visual Studio Code Dev containers extension to:

- download the required container images
- configure the docker environment
- configure the remote Visual Studio Code instance with the required extensions

> Note: no changes will be made to your Visual Studio Code installation; all changes will be applied to the VS Code instance running in the remote container

The repository `devcontainer` configuration will create:

- Container instances:
  - One RabbitMQ instance with management plugin support
  - One .NET-enabled container where the repository source code will be mapped
  - Four PostgreSQL instances (one per service domain: Marketing, Sales, Shipping, Warehouse)
- Configure the VS Code remote instance with:
  - The C# extension (`ms-dotnettools.csharp`)
  - The PostgreSQL Explorer extension (`ckolkman.vscode-postgres`)

Once the configuration is completed, VS Code will show a new `Ports` tab in the bottom-docked terminal area. The `Ports` tab will list all the ports the remote containers expose.

## Containers connection information

The default RabbitMQ credentials are:

- Username: `guest`
- Password: `guest`

The default PostgreSQL credentials are:

- User: `db_user`
- Password: `P@ssw0rd`

## How to run the demos

To execute the demo, open the repository root folder in VS Code, press `F1`, and search for `Reopen in container`. Wait for the Dev Container to complete the setup process.

Once the demo content has been reopened in the dev container, go to the `Run and Debug` VS Code section and select one of the available launch configurations:

- **`Demo - (build)`** – Builds the solution and then launches all services.
- **`Demo - (build & deploy data)`** – Builds the solution, creates and seeds the databases, and then launches all services. Use this option on the first run or after resetting the databases.
- **`Demo - (no build)`** – Launches all services without rebuilding first.

Each configuration launches all services simultaneously in integrated terminal windows:

- `Marketing.Api`, `Sales.Api`, `Shipping.Api`, `Warehouse.Api` – REST API endpoints
- `Marketing.Service`, `Sales.Service`, `Shipping.Service`, `Warehouse.Service` – background NServiceBus message-processing endpoints
- `WebApp` – the web frontend (opens automatically in the browser)

### Disclaimer

This demo is built using [NServiceBus Sagas](https://docs.particular.net/nservicebus/sagas/); I work for [Particular Software](https://particular.net/), the makers of NServiceBus.
