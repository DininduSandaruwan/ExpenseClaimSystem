# Expense Claim Management System

Project Overview
The Expense Claim Management System is a web-based application designed to manage employee expense claims efficiently. The system follows the Clean Architecture pattern to ensure separation of concerns, scalability, maintainability, and testability.
Architecture: Clean Architecture
The project is structured based on Clean Architecture principles. Each layer has a clear responsibility and depends only on inner layers.
1. Domain Layer (Core)
• Contains Entities (e.g., ExpenseClaim)
• Contains Enums (e.g., ClaimStatus)
• Business rules and core models
• No dependency on other layers
2. Application Layer
• Contains Interfaces (e.g., IExpenseClaimService)
• Contains DTOs and ViewModels
• Business logic implementation
• Depends only on Domain layer
3. Infrastructure Layer
• Entity Framework Core implementation
• Database context (AppDbContext)
• Repository implementations
• External services integration
• Implements interfaces defined in Application layer
4. Presentation Layer (Web)
• ASP.NET Core MVC Controllers
• Razor Views
• Bootstrap UI
• Handles HTTP requests and responses
• Depends on Application layer only
Technology Stack
•	.NET 10
•	ASP.NET Core MVC 10
•	Entity Framework Core 10
•	SQL Server 2022+
•	Bootstrap 5
•	Visual Studio 2026 Community Edition
•	C# 12
Prerequisites
•	.NET SDK 10.0+
•	SQL Server (LocalDB or Full Version)
•	SQL Server Management Studio (SSMS)
•	Visual Studio 2026 Community Edition
Database Setup
1. Run Migration with following command
EntityFrameworkCore\Update-Database -Project ExpenseClaimSystem.Infrastructure -StartupProject ExpenseClaimSystem.BlazorServer
Configuration
Update the connection string in appsettings.json with your SQL Server details.
Running the Application
•	Using Visual Studio:
• Open solution file
• Set as Startup Project
• Press F5 to run
Features
•	Create Expense Claim
•	Edit Expense Claim
•	Soft Delete Claim
•	Filter by Status, Department, and Date Range
•	View Claim Details in Bootstrap Modal
•	Enum-based Claim Status Handling
Default User Credentials (For Testing)
•	Admin User:
Email: admin@example.com
Password: Admin@123
•	Employee User:
Email: employee@example.com 
Password: Employee@123
Future Improvements
•	Export to Excel/PDF
•	Email Notifications
•	Approval Workflow
•	Dashboard Analytics
•	Pagination and Sorting Improvements
