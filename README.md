# ??? Mess Management System

A comprehensive ASP.NET Core MVC application for managing hostel/mess operations with attendance tracking, menu management, and automated billing.

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)
[![Deploy](https://img.shields.io/badge/deploy-Render-purple)](https://render.com)

## ?? Features

### ?? User Management
- **Role-based Access Control** (Admin, Student)
- **Secure Authentication** with ASP.NET Identity
- **Gmail-only Registration** with validation
- **Profile Management** with password change

### ?? Menu Management
- Create and manage menu items
- Category-based organization (Beverage, Main Course, Side Dish, Dessert, Snack)
- Effective date control for menu availability
- Mess group assignment (Mandatory/Optional)

### ? Attendance Tracking
- Daily attendance marking by admin
- **Item-by-item selection** during attendance
- Real-time cost calculation
- Historical attendance records
- **Price locking** at time of selection

### ?? Billing System
- **100% Accurate Billing** based on actual consumption
- Automatic bill generation
- Separate tracking for Food and Water/Tea charges
- **Incremental bill updates** (regenerate anytime)
- Bill approval workflow
- Payment tracking

### ?? Dashboard & Reports
- Admin dashboard with statistics
- Student dashboard with personal data
- Daily bill view for students
- Monthly bill generation
- Payment status tracking

## ??? Architecture

### Technology Stack
- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: 
  - SQL Server (Local Development)
  - PostgreSQL (Production on Render)
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Identity
- **Frontend**: Bootstrap 5, jQuery, Font Awesome

### Design Patterns
- **Repository Pattern** with Unit of Work
- **Dependency Injection**
- **ViewModel Pattern** for clean separation
- **Attribute-based Validation**

### Database Schema
```
Users (AspNetUsers)
??? Roles (Admin, Student)
??? UserMessGroups
??? Attendances
?   ??? AttendanceMenuItems (Junction table)
??? Bills
?   ??? BillDetails
?   ??? Payments
??? Menus
    ??? MessGroups
```

## ?? Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB for development)
- [Git](https://git-scm.com/)

### Local Development

1. **Clone the repository**
```bash
git clone https://github.com/2022cs668-Waleed/Mess-management-system.git
cd Mess-management-system
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Update database**
```bash
dotnet ef database update
```

4. **Run the application**
```bash
dotnet run
```

5. **Access the application**
```
https://localhost:5001
```

### Default Credentials

**Admin:**
- Email: `admin@gmail.com`
- Password: `Admin@123`

**Students:**
- Email: `student1@gmail.com` / Password: `Student@123`
- Email: `student2@gmail.com` / Password: `Student@123`
- Email: `student3@gmail.com` / Password: `Student@123`

## ?? Deployment

### Deploy to Render

This application is configured for one-click deployment on Render.

1. **Fork this repository**

2. **Create Render account** at [render.com](https://render.com)

3. **Deploy using Blueprint**
   - New ? Blueprint Instance
   - Connect your repository
   - Render will create:
     - PostgreSQL database
     - Web service
     - Environment variables

4. **Access your deployed app**
```
https://your-app-name.onrender.com
```

For detailed instructions, see [RENDER_DEPLOYMENT_GUIDE.md](RENDER_DEPLOYMENT_GUIDE.md)

### Quick Deploy
See [QUICK_DEPLOY.md](QUICK_DEPLOY.md) for 5-minute deployment guide.

## ?? Documentation

- [Render Deployment Guide](RENDER_DEPLOYMENT_GUIDE.md)
- [Quick Deploy Guide](QUICK_DEPLOY.md)
- [Reset Complete Documentation](RESET_COMPLETE.md)
- [Incremental Bill Generation](INCREMENTAL_BILL_GENERATION_COMPLETE.md)

## ?? Key Features Explained

### Attendance-Based Item Selection
Instead of assuming fixed daily charges, admins select specific menu items for each student during attendance marking. This ensures:
- ? No assumptions about consumption
- ? 100% accurate billing
- ? Complete transparency
- ? Price protection (prices locked at selection time)

### Incremental Bill Generation
Bills can be regenerated multiple times within a month:
- First generation: Creates new bills
- Subsequent generations: Updates existing bills with new data
- No duplicate bills possible
- Always reflects current attendance data

### Effective Date Control
Menu items have effective dates:
- Items only appear in attendance marking from their effective date onwards
- Allows planning future menu changes
- Historical items remain in past records

## ??? Development

### Project Structure
```
Mess-Management-System/
??? Controllers/        # MVC Controllers
??? Models/            # Domain models
??? ViewModels/        # View-specific models
??? Views/             # Razor views
??? Data/              # DbContext and seed data
??? Repositories/      # Data access layer
??? Attributes/        # Custom validation attributes
??? wwwroot/           # Static files
??? Migrations/        # EF Core migrations
??? Properties/        # Launch settings
```

### Adding Migrations
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Running Tests
```bash
dotnet test
```

## ?? Database Seeding

On first run, the application automatically seeds:
- 1 Admin account
- 3 Student accounts
- 2 Mess Groups (Water & Tea, Food)
- 7 Menu Items with prices

## ?? Security Features

- ? Password hashing with Identity
- ? Anti-forgery tokens
- ? Role-based authorization
- ? Secure cookies in production
- ? HTTPS enforcement in production
- ? Gmail-only registration
- ? Unique email validation

## ?? Use Cases

### For Admins
1. Manage users (create, activate, deactivate)
2. Create and manage menu items
3. Mark daily attendance with item selection
4. Generate monthly bills
5. Approve bills
6. Track payments

### For Students
1. View menu items and prices
2. Check daily bill breakdown
3. View monthly bills
4. Track payment status

## ?? License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## ?? Contributors

- Waleed Ahmed (2022-CS-668)

## ?? Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ?? Support

For support, email your.email@gmail.com or open an issue in the repository.

## ?? Acknowledgments

- ASP.NET Core team for the amazing framework
- Bootstrap team for the UI framework
- Font Awesome for icons
- Render for hosting platform

---

**Made with ?? using ASP.NET Core**
