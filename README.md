# Tracker.Core.Api

Tracker.Core.Api is a backend API for managing and tracking expenses. Built with .NET, this project focuses on creating a structured and efficient platform for financial tracking, leveraging modern technologies.

## Features
- **Transaction Management**: Track income and expenses with structured data models.
- **Category Support**: Organize transactions into user-defined categories.
- **Data Insights**: Tools to analyze financial trends (planned for future releases).
- **Scalable Architecture**: Designed to support future integrations and scalability.

## Getting Started

### Prerequisites

To set up the project, ensure you have the following:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A database system compatible with EF Core (e.g., SQL Server)
- Git for version control

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/pratikkaje/Tracker.Core.Api.git
   ```
2. Navigate to the project directory:
   ```bash
   cd Tracker.Core.Api
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Set up the database:
   - Update the connection string in `appsettings.json`.
   - Apply migrations:
     ```bash
     dotnet ef database update
     ```

5. Run the application:
   ```bash
   dotnet run
   ```

## Project Structure

- **Models**: Define core entities such as `User`, `Category`, and `Transaction`.
- **Data**: Contains EF Core migrations and context configuration.
- **Services**: Business logic for managing transactions and categories.
- **Controllers**: API endpoints for interaction with the application.

## API Endpoints

The following endpoints are available:

- `/api/transactions`: Manage income and expense transactions.
- `/api/categories`: CRUD operations for categories.

Detailed API documentation is in progress.

## Contributing

We welcome contributions! To contribute:

1. Fork the repository.
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Description of changes"
   ```
4. Push to your fork and create a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Thanks to all contributors and open-source libraries that made this project possible.
- Inspired by common financial tracking needs.
