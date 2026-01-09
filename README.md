# TankR 🚗⛽

TankR is a backend-driven fuel station management system designed to handle stations, users, and fuel transactions in a secure and scalable way. The project follows an API-first approach and is built for extensibility, making it suitable for both academic and real-world use cases.

## Features
- Role-based access control (Admin, Cashier, Customer)
- JWT authentication and authorization
- Fuel station and transaction management
- User profile and address management
- Email notifications (registration, password reset)
- Station logo support via image URLs
- Dockerized setup for consistent development and deployment
- PostgreSQL relational database with normalized schema
- Swagger/OpenAPI documentation

## Tech Stack
- **Backend:** ASP.NET Core Web API (C#)
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL
- **Authentication:** JWT
- **Containerization:** Docker & Docker Compose
- **Version Control:** Git & GitHub

## Architecture
TankR follows a layered architecture:
- Controllers – handle HTTP requests
- Repositories/Services – business logic and data access
- Data Layer – entities and database context

## Getting Started
1. Clone the repository
2. Configure environment variables
3. Run the project using Docker:
   ```bash
   docker-compose up --build


---

## Author

**Artian Rika**  
© 2026 — TankR Backend  
GitHub: [@artianRika](https://github.com/artianRika)