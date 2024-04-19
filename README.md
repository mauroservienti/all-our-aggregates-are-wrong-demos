# All our Aggregates are Wrong - Demos

A microservices-powered e-commerce shopping cart sample based on SOA principles. These are the demos for my [All our Aggregates are Wrong](https://milestone.topics.it/talks/all-our-aggregates-are-wrong.html) talk.

The demo demonstrates a shopping cart behavior and all its implemented functionalities. Add items to the cart and observe the various "services" console windows, which display log messages related to the ongoing processes. Leave the cart inactive for a few seconds and observe the stale cart policy kick in, first raising a warning and finally deleting stale carts.

## Requirements

The following requirements must be met to run the demos successfully:

- [Visual Studio Code](https://code.visualstudio.com/) and the [Dev containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers).
- [Docker](https://www.docker.com/get-started) must be pre-installed on the machine.
- The repository `devcontainer` setup requires `docker-compose` to be installed on the machine.

## How to configure Visual Studio Code to run the demos

- Clone the repository
  - On Windows, make sure to clone on a short path, e.g., `c:\dev`, to avoid any "path too long" error
- Open one of the demo folders in Visual Studio Code
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

- One or more container instances:
  - One RabbitMQ instance with management plugin support
  - One .NET-enabled container where the repository source code will be mapped
  - A few PostgreSQL instances
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

To execute the demo, open the root folder in VS Code, press `F1`, and search for `Reopen in container`. Wait for the Dev Container to complete the setup process.

Once the demo content has been reopened in the dev container:

1. Press `F1`, search for `Run task`, and execute the desired task to build the solution or to build the solution and deploy the required data
2. Go to the `Run and Debug` VS Code section and select the command you want to execute.

### Disclaimer

This demo is built using [NServiceBus Sagas](https://docs.particular.net/nservicebus/sagas/); I work for [Particular Software](https://particular.net/), the makers of NServiceBus.
