# All our Aggregates are Wrong - Demos

A microservices powered e-commerce shopping cart sample - based on SOA principles. These are the demos for my [All our Aggregates are Wrong](https://milestone.topics.it/talks/all-our-aggregates-are-wrong.html) talk.

The demo demoes a shopping cart behavior and all its implemented functionalities. Add items to the cart, observe the various "services" console windows they'll display log messages related to the ongoing processes. Leave the cart inactive for a few seconds and observe the stale cart policy kicking in, first raising a warning, finally deleting stale carts.

## How to get the sample working locally

### Get a copy of this repository

Clone or download this repo locally on your machine.

> [!NOTE]
> On Windows, If you're downloading a zip copy of the repo please make sure the zip file is unblocked before decompressing it. In order to unblock the zip file:
>
> - Right-click on the downloaded copy
> - Choose Property
> - On the Property page tick the unblock checkbox
> - Press OK

### Requirements

- .NET 7 or later
- A SQL Server edition or SQL Server running in a Docker container.

The demo uses the following connection string to connect to the various required SQL databases:

```sql
Data Source=.;Initial Catalog=Sales;User Id=sa;Password=YourStrongPassw0rd;TrustServerCertificate=True
```

Adjust it to your needs.

### Databases creation

The demo expects the following databases to be available:

- Marketing
- Sales
- Shipping
- Warehouse

The [scripts](scripts) folder in this repository contains a simple `Setup-Databases.sql` script that creates the required databases.

### Createing the default dataset

Run the `CreateRequiredDatabases` project to fill the created databases with a default dataset to run the demos.

## Startup projects

Ensure the following projects are set as startup projects to run the default setup:

- WebApp
- Sales.Api
- Marketing.Api
- Warehouse.Api
- Shipping.Api
- Sales.Service
- Marketing.Service
- Warehouse.Service
- Shipping.Service

## Demo (failed services)

Demoes what happens when a back-end service is not available. Add an item to the cart, visualize the shopping cart and observe the different information displayed in relation to the shipping estimates. Ensure the following projects are set as startup projects:

- WebApp
- Sales.Api
- Marketing.Api
- Warehouse.Api
- Shipping.Api
- Sales.Service
- Marketing.Service

### Demo (Platform) — Windows only, at the moment

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
