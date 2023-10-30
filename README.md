# All our Aggregates are Wrong - Demos

A microservices powered e-commerce shopping cart sample - based on SOA principles. These are the demos for my [All our Aggregates are Wrong](https://milestone.topics.it/talks/all-our-aggregates-are-wrong.html) talk.

## How to get the sample working locally

### Get a copy of this repository

Clone or download this repo locally on your machine. If you're downloading a zip copy of the repo please be sure the zip file is unblocked before decompressing it. In order to unblock the zip file:

- Right-click on the downloaded copy
- Choose Property
- On the Property page tick the unblock checkbox
- Press OK

### Check your machine is correctly configured

In order to run the sample the following machine configuration is required:

- PowerShell execution policy to allow script execution, from an elevated PowerShell run the following:

```powershell
Set-ExecutionPolicy Unrestricted
```

- Visual Studio 2019 with [.NET 5](https://dotnet.microsoft.com/download/dotnet/5.0) support (Community Edition is supported), available for download at [https://www.visualstudio.com/downloads/](https://www.visualstudio.com/downloads/)

- A SQL Server edition or the `LocalDb` instance installed by Visual Studio, in case of a clean machine with `LocalDb`only please install:
  - Microsoft ODBC Driver 11 for SQL Server, available for download at [https://www.microsoft.com/en-us/download/details.aspx?id=36434](https://www.microsoft.com/en-us/download/details.aspx?id=36434)
  - Microsoft ODBC Command Line Utilities 11 for SQL Server, available for download at [https://www.microsoft.com/en-us/download/details.aspx?id=36433](https://www.microsoft.com/en-us/download/details.aspx?id=36433)

NOTE: On a clean machine do not install the latest version, as of this writing 13.1, of Microsoft ODBC Driver and Microsoft ODBC Command Line Utilities as the latter is affected by a bug that prevents the `LocalDb` instance to be accessible at configuration time.

### Databases setup

To simplify `LocalDB` instance setup 2 PowerShell scripts, in the [scripts](scripts) folder, are provided for your convenience. Both need to be run from an elevated PowerShell console.

- Run `Setup-Databases.ps1`, with elevation, to create the `LocalDB` instance and all the required databases
- Run `Teardown-Databases.ps1`, with elevation, to drop all the databases and delete the `LocalDB` instance

The created `LocalDB` instance is named `(localdb)\all-our-aggregates-are-wrong`.

NOTE: If you receive errors regarding "Microsoft ODBC Driver", you can work around these by connecting to the `(localdb)\all-our-aggregates-are-wrong` database using, for example, Visual Studio or SQL Management Studio, and running the SQL contained in the `Setup-Databases.sql` file to manually create databases.

NOTE: In case the database setup script fails with a "sqllocaldb command not found" error it is possible to install `LocalDb` as a standalone package by downloading it separately at [https://www.microsoft.com/en-us/download/details.aspx?id=29062](https://www.microsoft.com/en-us/download/details.aspx?id=29062)

## Startup projects

Solutions are configured to use the [SwitchStartupProject](https://marketplace.visualstudio.com/items?itemName=vs-publisher-141975.SwitchStartupProject) Visual Studio Extension to manage startup projects. The extension is not a requirement, it's handy.

The solution can be configured to run 3 different demos.

### Demo (default)

Demos the default shopping cart behavior and all its implemented functionalities. Add items to the cart, observe the various "services" console windows they'll display log messages related to the ongoing processes. Leave the cart inactive for a few seconds and observe the stale cart policy kicking in, first raising a warning, finally deleting stale carts. Ensure the following projects are set as startup projects:

- WebApp
- Sales.Api
- Marketing.Api
- Warehouse.Api
- Shipping.Api
- Sales.Service
- Marketing.Service
- Warehouse.Service
- Shipping.Service

### Demo (failed services)

Demos what happens when a back-end service is not available. Add an item to the cart, visualize the shopping cart and observe the different information displayed in relation to the shipping estimates. Ensure the following projects are set as startup projects:

- WebApp
- Sales.Api
- Marketing.Api
- Warehouse.Api
- Shipping.Api
- Sales.Service
- Marketing.Service

### Demo (Platform)

Uses the [Particular Platform](https://particular.net/service-platform) Sample package to visualize monitoring information, and messages and policies (Sagas) runtime behaviors.

- WebApp
- Sales.Api
- Marketing.Api
- Warehouse.Api
- Shipping.Api
- Sales.Service
- Marketing.Service
- Warehouse.Service
- Shipping.Service
- PlatformLauncher

## NServiceBus configuration

This sample has no [NServiceBus](https://particular.net/nservicebus) related pre-requisites as it's configured to use [Learning Transport](https://docs.particular.net/nservicebus/learning-transport/) and [Learning Persistence](https://docs.particular.net/nservicebus/learning-persistence/), both explicitly designed for short term learning and experimentation purposes.

They should also not be used for longer-term development, i.e. the same transport and persistence used in production should be used in development and debug scenarios. Select a production [transport](https://docs.particular.net/transports/) and [persistence](https://docs.particular.net/persistence/) before developing features. 

> NOTE: Do not use the learning transport or learning persistence to perform any kind of performance analysis.

### Disclaimer

Parts of this demo are built using [NServiceBus](https://particular.net/nservicebus), I work for [Particular Software](https://particular.net/), the makers of NServiceBus.
