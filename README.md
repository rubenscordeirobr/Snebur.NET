# Snebur

Snebur is a personal project that I am developing during my free time and weekends.

This project primarily focuses to base for other project, 
using tenants, users, userSessions, and activity management,

The entities chages will be tracked in the activity collection, and the messages will be stored in a separate collection,
 
## Technologies and Architecture

It will follow the Clean Architecture pattern using C# .NET 10.0


### **Backend Architecture Overview:**
- **Identity ** -> PostgreSQL Database
- **Activities** -> MongoDB
- **Caching Service** -> Redis


## Project Structure

The project is organized into multiple layers and services following Clean Architecture principles:

### 1. Shared Layers

### Snebur.Core
This project serves as a DRY (Don't Repeat Yourself) library containing common utilities:

- **Extensions** Utility extension methods
- **Helpers** Complex utilities that depend on other dependencies (e.g., `PasswordHelper` using SHA256)
- **Utils** Pure functions without dependencies
- 
#### Snebur.SharedKernel

- **Purpose:**  
  A dedicated layer for **cross-cutting concerns** shared across the entire solution. It is independent of any specific layer and ensures compliance with the **Dependency Rule**.

- **Key Features:**
  - **Validation Constants:** Centralized constants, such as maximum string lengths and business rule constraints.
  - **Enums:** Shared enumerations used across multiple layers.
  - **Abstractions:** Shared Abstractions that multiple layers may implement.

  - **Structure:**
  
  - This project contains only **abstractions**, records, enums, and interfaces.
  - No methods or utility classes are included in this layer.
  - This layer should not reference any other project in the solution.

- **Why SharedKernel?**
  - Keeps shared logic reusable and decoupled.
  - Prevents duplication of constants and enums across layers.
  - Maintains clean separation of concerns.

### **2. Core Layer**

#### Snebur.Domain
- **Constants** Domain-specific constants
- **Entities** Core business entities
- **Enums** Enumerations related to the domain
- **Extensions** Domain extension methods
- **ValueObjects** Immutable domain value objects

#### Snebur.Application
- **Purpose:**  
  This layer contains application-specific, abstractions and mediator for commands, queries, and domain events. It serves as a bridge between the Domain and Infrastructure layers.
- **Abstractions**
  - Notifications
  - Persistence
  - Security
  - Services
- **Exceptions** Custom exceptions for error handling
- **Validations** Input and business rule validations

#### Snebur.UseCases
- **Purpose:**  
  This layer contains application-specific use cases, including backend implementations for commands, queries, and domain events.

- - **Identities** Handles identity-related operations
- **Activities** Business logic for activities
- **Messages** Message handling and processing

#### Snebur.UseCases.Shared

- **Purpose:**  
  This layer contains shared use cases, including DTOs for requests and responses, as well as validators. These components are designed for use in both the frontend and backend.

- **Key Features:**
  - **Decoupling:** Enables consistent data handling and validation across Blazor and React UIs without exposing the Domain layer.
  - **DTOs:** Data Transfer Objects used for API communication.
  - **Validation Logic:** Centralized validation rules for input data using FluentValidation or similar libraries.
  - **Interfaces** Interfaces ensuring integrity, such as Automapper
  - **Abstractions** Services that will be used by the UI to communicate with the backend
 
- **Structure:**
  - This project contains **DTOs**, **validation classes**, **interfaces**, and **Abstractions**.
  - This project should reference Snebur.SharedKernel and Snebur.Core, but no other project in the solution.
  

### **3. Infrastructure Layer**

#### Snebur.Infrastructure
- **CacheServices** Services responsible for caching operations
- **EmailServices** Services for sending emails
- **NotificationServices** Services for sending notifications

#### Application.Persistence
- **Database** PostgreSQL database for entities management
- **Repositories** Data access layer for entities
- **Migrations** Database migrations
- **Configurations** Database configurations
- **DbContext** Entity Framework DbContext

#### Snebur.Persistence.Activities
- **Database** MongoDB database for activity storage
 