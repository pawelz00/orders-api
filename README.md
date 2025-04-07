# Orders API

This repository contains a .NET 9.0 based Orders API application. The solution is structured into multiple projects, each serving a specific purpose within the application.

## Projects

### 1. Domain
This project contains the core domain models and interfaces. It defines the entities and their relationships, as well as the repository interfaces.

### 2. Application
This project contains the application logic and services. It references the Domain project and implements the business logic using the domain models and interfaces.

### 3. Infrastructure
This project contains the data access layer and repository implementations. It references the Application project and uses Entity Framework Core to interact with the database.

### 4. API
This project contains the API controllers and endpoints. It references the Application project and exposes the functionality via HTTP endpoints.

## Project Structure

- **Domain**: Contains the domain models (`Order`, `Product`, `OrderItem`) and repository interfaces (`IOrderRepository`, `IProductRepository`).
- **Application**: Contains the application services and interfaces for interacting with the domain models.
- **Infrastructure**: Contains the repository implementations (`OrderRepository`, `ProductRepository`) and the database context (`ApplicationDbContext`).
- **API**: Contains the API controllers that expose the endpoints for managing orders and products.

## API Endpoints

### Orders

- **GET /api/orders**: Retrieves all orders.
- **GET /api/orders/{id}**: Retrieves a specific order by ID.
- **POST /api/orders**: Creates a new order.
- **PUT /api/orders/{id}**: Updates an existing order by ID.
- **DELETE /api/orders/{id}**: Deletes an order by ID.
- **POST /api/orders/{orderId}/add-items**: Adds products to an order.
- **DELETE /api/orders/{orderId}/remove-items**: Deletes products from an order.

### Products

- **GET /api/products**: Retrieves all products.
- **GET /api/products/{id}**: Retrieves a specific product by ID.
- **POST /api/products**: Creates a new product.
- **PUT /api/products/{id}**: Updates an existing product by ID.
- **DELETE /api/products/{id}**: Deletes a product by ID.

## How Projects Work Together

1. **Domain**: Defines the core entities and repository interfaces.
2. **Application**: Implements the business logic and uses the domain models and interfaces.
3. **Infrastructure**: Implements the repository interfaces using Entity Framework Core and provides the data access layer.
4. **API**: Exposes the application functionality via HTTP endpoints and interacts with the application services.

------------

# CI/CD Pipeline for Orders API

GitHub Actions automates the build and deployment of the Orders API to an Azure Web App.

## Workflow Overview
This workflow:
1. Triggers on pushes to the `master` branch or manual dispatch.
2. Builds the .NET project using `.NET 9.x`.
3. Publishes the API to a specified directory.
4. Uploads the build artifacts.
5. Deploys the application to Azure Web App.

## Environment Variables
- `AZURE_WEBAPP_NAME`: Name of the Azure Web App.
- `AZURE_WEBAPP_PACKAGE_PATH`: Path to the published app.
- `DOTNET_VERSION`: .NET version to use.
- `SOLUTION_PATH`: Path to the .NET solution file.
- `API_PATH`: Path to the API project.
- `PUBLISH_DIR`: Directory for the published output.

## Secrets
- `AZURE_WEBAPP_PUBLISH_PROFILE`: Azure publish profile for authentication.

## Job Breakdown
### Build Job
1. Checks out the repository.
2. Sets up .NET Core.
3. Restores dependencies.
4. Builds the solution in `Release` mode.
5. Runs tests.
6. Publishes the API.
7. Uploads the published artifacts.

### Deploy Job
1. Downloads the published artifact.
2. Deploys it to the Azure Web App

------------

# Azure

Web API is deployed in Azure (Azure App Service) and uses Azure SQL Database.

- Azure Web App: Hosts and runs the application.
- Azure SQL Database: Stores application data.
- Connection Strings: Used to connect the web application to the database.
