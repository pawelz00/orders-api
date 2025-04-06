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
- **POST /api/orders/{orderId}/products**: Adds products to an order.
- **DELETE /api/orders/{orderId}/products**: Deletes products from an order.

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
