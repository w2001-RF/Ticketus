# Ticketus Backend

This is the backend API for the Ticketus web application built using .NET 8. The API provides endpoints for user authentication, ticket management, and user settings.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server or any compatible database
- Postman or any API testing tool (optional)

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone https://github.com/w2001-RF/Ticketus.git
   cd ticketus-backend
2. **Restore Dependencies**
 ```bash
   dotnet restore
3. **Configure Database**
   - Update the connection string in `appsettings.json` to match your SQL Server setup:
   `{
       "ConnectionStrings": {
           "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
       }
   }`

4. **Run Migrations**
 ```bash
   dotnet ef database update

5. **Run the Application**
 ```bash
   dotnet run

## Setup Instructions

To run unit tests, use the following command:
```bash
   dotnet test