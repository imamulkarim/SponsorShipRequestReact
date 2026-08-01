# Tech Assessment - Sponsorship Management System

A comprehensive sponsorship request management system built with .NET 10 (Clean Architecture) and Angular, featuring role-based approvals workflow for managers and finance administrators.

## Table of Contents

- [Overview](#overview)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Setup Guide](#setup-guide)
- [Running the Application](#running-the-application)
- [Test Login Accounts](#test-login-accounts)
- [API Documentation](#api-documentation)
- [Troubleshooting](#troubleshooting)
- [Development Workflow](#development-workflow)

---

## Overview

The Sponsorship Management System is designed to streamline the sponsorship request approval workflow with the following key features:

- **Requestor Dashboard**: Users can create, submit, and track sponsorship requests
- **Manager Approval**: Managers can review and approve/reject requests at the department level
- **Finance Approval**: Finance administrators can approve/reject requests at the organizational level
- **Admin Dashboard**: System administrators can view all requests and monitor workflow history
- **Role-Based Access**: Built-in role-based authorization (Requestor, Manager, FinanceAdmin, SystemAdmin)
- **Clean Architecture**: Organized with Domain, Application, Infrastructure, and Web layers

The project was generated using the [Clean.Architecture.Solution.Template](https://github.com/jasontaylordev/CleanArchitecture) version 10.8.0.

---

## Technology Stack

### Backend
- **.NET 10** with ASP.NET Core
- **Entity Framework Core** with PostgreSQL
- **MediatR** for CQRS pattern
- **AutoMapper** for DTO mapping
- **Swagger/OpenAPI** for API documentation
- **ASP.NET Identity** for authentication
- **.NET Aspire** (optional) for orchestration

### Frontend
- **Angular 18+** (TypeScript)
- **RxJS** for reactive programming
- **Bootstrap/CSS** for styling
- **NSwag** for TypeScript client generation

### Database
- **PostgreSQL** (primary database)
- **Entity Framework Core** for ORM

---

## Project Structure

```
TechAssessment/
├── src/
│   ├── Web/                          # ASP.NET Core Web API
│   │   ├── Program.cs               # Application startup
│   │   ├── Endpoints/               # Minimal API endpoints
│   │   ├── Infrastructure/          # Web-specific infrastructure
│   │   └── ClientApp/               # Angular frontend
│   │       ├── src/
│   │       │   ├── app/
│   │       │   │   ├── models/      # TypeScript models
│   │       │   │   ├── services/    # Angular services
│   │       │   │   ├── sponsorship/ # Feature modules
│   │       │   │   └── web-api-client.ts  # Generated client
│   │       │   └── main.ts
│   │       ├── angular.json
│   │       ├── package.json
│   │       └── nswag.json          # API client generation config
│   ├── Application/                 # Business logic (CQRS)
│   │   ├── Commands/               # Command handlers
│   │   └── Queries/                # Query handlers
│   ├── Domain/                      # Domain entities
│   │   ├── Entities/               # Business domain models
│   │   ├── Events/                 # Domain events
│   │   └── Enums/
│   ├── Infrastructure/              # Data access & external services
│   │   ├── Data/                   # EF configurations
│   │   └── Identity/               # Authentication setup
│   └── Shared/                      # Shared DTOs & utilities
├── AppHost/                         # .NET Aspire orchestration (optional)
└── ServiceDefaults/                 # Aspire service defaults
```

---

## Setup Guide

### Prerequisites

Before starting, ensure you have the following installed:

1. **.NET 10 SDK**
   - Download from: https://dotnet.microsoft.com/download/dotnet/10.0
   - Verify installation: `dotnet --version`

2. **PostgreSQL** (version 12 or higher)
   - Download from: https://www.postgresql.org/download/
   - Verify installation: `psql --version`
   - Ensure PostgreSQL service is running

3. **Node.js & npm** (for Angular development)
   - Download from: https://nodejs.org/
   - Verify installation: `node --version` and `npm --version`

4. **Visual Studio Community 2026** (or VS Code)
   - Recommended for .NET development

5. **Git** (for version control)
   - Download from: https://git-scm.com/

### Step 1: Clone the Repository

```bash
git clone <repository-url>
cd TechAssessment
```

### Step 2: Database Setup

#### Create PostgreSQL Database

1. **Open PostgreSQL Command Line:**
   ```bash
   psql -U postgres
   ```

2. **Create database:**
   ```sql
   CREATE DATABASE tech_assessment;
   ```

3. **Create dedicated user (optional but recommended):**
   ```sql
   CREATE USER assessment_user WITH PASSWORD 'Assessment@123';
   ALTER ROLE assessment_user SET client_encoding TO 'utf8';
   ALTER ROLE assessment_user SET default_transaction_isolation TO 'read committed';
   GRANT ALL PRIVILEGES ON DATABASE tech_assessment TO assessment_user;
   ```

#### Configure Connection String

Update `src/Web/appsettings.json` with your database credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tech_assessment;Username=assessment_user;Password=Assessment@123"
  }
}
```

Or if using default postgres user:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tech_assessment;Username=postgres;Password=<your-postgres-password>"
  }
}
```

### Step 3: Initialize Database with EF Core Migrations

```bash
cd src/Web

# Apply migrations and create database schema
dotnet ef database update

# Or from repository root:
dotnet ef database update --project src/Web
```

This will:
- Create all database tables
- Seed initial roles (Requestor, Manager, FinanceAdmin, SystemAdmin)
- Set up ASP.NET Identity tables
- Initialize test user accounts

### Step 4: Angular Frontend Setup

```bash
cd src/Web/ClientApp

# Install npm dependencies
npm install

# (Optional) Generate/regenerate TypeScript client from Swagger
npm run generate-client
```

### Step 5: Build the Solution

```bash
# From repository root
dotnet build

# Expected output: Build succeeded
```

---

## Running the Application

### Option 1: Direct WebAPI + Angular (Recommended for Development)

This approach runs the backend and frontend separately, giving you maximum control and faster feedback during development.

#### Terminal 1: Start Backend WebAPI

```bash
cd src/Web

# Run the backend with hot reload
dotnet run

# Expected output:
# info: Microsoft.Hosting.Lifetime[14]
#       Now listening on: https://localhost:7081
#       Now listening on: http://localhost:5000
```

**Verify Backend is Running:**
- API Health: https://localhost:7081/health (if configured)
- Swagger UI: https://localhost:7081/swagger/ui
- API Base URL: https://localhost:7081/api

#### Terminal 2: Start Angular Frontend

```bash
cd src/Web/ClientApp

# Start Angular development server with hot reload
npm start

# Expected output:
# ✔ Compiled successfully.
# ✔ Compiled successfully in 5.23s.
# ○ Generating browser application bundles...
# 
# ✔ Build successful.
```

**Verify Frontend is Running:**
- Application URL: http://localhost:4200
- DevTools: Press F12 to check console for errors

#### Access the Application

1. Open browser to `http://localhost:4200`
2. You'll be redirected to login page
3. Enter test credentials (see [Test Login Accounts](#test-login-accounts))
4. Login and navigate through the application

#### Stop the Services

- **Backend**: Press `Ctrl + C` in Terminal 1
- **Frontend**: Press `Ctrl + C` in Terminal 2

---

### Option 2: Using .NET Aspire Orchestration

This approach uses .NET Aspire to orchestrate all services (database, API, frontend) with centralized dashboard.

#### Prerequisites for Aspire

1. **Install Aspire workload** (if not already installed):
   ```bash
   dotnet workload update
   dotnet workload install aspire
   ```

2. **Verify Aspire installation:**
   ```bash
   dotnet workload list
   # Should show: aspire (active)
   ```

#### Step 1: Verify Aspire Configuration

Ensure `AppHost/Program.cs` contains:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .AddDatabase("techassessment");

var api = builder.AddProject<Projects.TechAssessment_Web>("api")
    .WithReference(postgres)
    .WaitFor(postgres);

builder.AddNpmApp("frontend", "../src/Web/ClientApp")
    .WithReference(api)
    .WaitFor(api)
    .WithEnvironment("API_BASE_URL", "http://localhost:5000");

builder.Build().Run();
```

#### Step 2: Run with Aspire

```bash
# From repository root
dotnet run --project .\AppHost

# Expected output:
# info: Aspire.Hosting[0]
#       Aspire dashboard available at https://localhost:18888
#
# Starting resource "postgres"...
# Starting resource "api"...
# Starting resource "frontend"...
```

#### Step 3: Access Aspire Dashboard

1. Open browser to `https://localhost:18888`
2. **Dashboard Features:**
   - **Resources Tab**: Shows all running services (postgres, api, frontend)
   - **Logs Tab**: Real-time logs from each service
   - **Metrics Tab**: Performance monitoring (CPU, memory, requests)
   - **Traces Tab**: Distributed tracing for API calls

#### Service URLs (with Aspire)

- **Aspire Dashboard**: https://localhost:18888
- **Frontend**: http://localhost:4200 (shown in dashboard)
- **API**: https://localhost:7081 (shown in dashboard)
- **Swagger UI**: https://localhost:7081/swagger/ui
- **PgAdmin**: http://localhost:5050 (database admin)

#### Viewing Logs in Aspire

1. Open Aspire Dashboard: https://localhost:18888
2. Click on a resource (api, frontend, postgres)
3. Click "Logs" to see real-time output
4. Search/filter logs by service or message

#### Stop Aspire

Press `Ctrl + C` in the terminal to stop all services.

---

## Test Login Accounts

### Pre-seeded Test Users

The database initialization script automatically creates the following test users. Use these to test different role-based workflows:

| Username | Password | Role(s) | Purpose |
|----------|----------|---------|---------|
| `requestor@example.com` | `Requestor@123` | Requestor | Create and submit sponsorship requests |
| `manager@example.com` | `Manager@123` | Manager | Approve/reject manager-level requests |
| `finance@example.com` | `Finance@123` | FinanceAdmin | Approve/reject finance-level requests |
| `admin@example.com` | `Admin@123` | SystemAdmin | View all requests, monitor workflow |

### Login Steps

1. Navigate to `http://localhost:4200`
2. Enter credentials from table above
3. Click **Sign In** button
4. Application redirects to role-specific dashboard

### Feature Access by Role

#### Requestor (`requestor@example.com`)
- **Dashboard**: View own submitted sponsorship requests
- **Create New**: Create new sponsorship requests
- **Edit Draft**: Modify requests in draft status
- **Submit Request**: Submit draft requests for approval
- **Track Status**: Monitor approval progress through workflow
- **Cancel Request**: Cancel pending or rejected requests

#### Manager (`manager@example.com`)
- **Manager Approvals**: View requests pending manager approval
- **Approve/Reject**: Make approval decisions with optional comments
- **View Details**: See full request details before deciding
- **Workflow History**: Track approval workflow for each request
- **Cannot**: Create requests or access finance approval

#### Finance Admin (`finance@example.com`)
- **Finance Approvals**: View requests pending finance approval
- **Approve/Reject**: Make final finance approval decisions
- **View Details**: See full request details including manager comments
- **Workflow History**: View complete approval workflow
- **Cannot**: Create requests or access manager approval

#### System Admin (`admin@example.com`)
- **Admin Dashboard**: View ALL requests in the system
- **Workflow History**: View complete workflow history for any request
- **System Access**: Full system access and monitoring
- **Reports Ready**: System prepared for admin reporting (future feature)

### Testing Workflow

To test the complete approval workflow:

1. **Login as Requestor**
   - Create a new sponsorship request
   - Fill in all details (title, department, type, amount, etc.)
   - Click "Save as Draft"
   - Click "Submit Request"

2. **Login as Manager**
   - Go to Manager Approvals
   - Find the submitted request
   - Click to review details
   - Click "Approve" with optional comment
   - Return to dashboard to confirm submission

3. **Login as Finance Admin**
   - Go to Finance Approvals
   - Find the manager-approved request
   - Click to review details and manager comments
   - Click "Approve" to finalize
   - Return to dashboard

4. **Login as Admin**
   - Go to Admin Dashboard
   - Find the request you just processed
   - Click to view complete workflow history
   - See all approvals and comments

---

## API Documentation

### Swagger UI

Interactive API documentation available at:

```
https://localhost:7081/swagger/ui
```

**Swagger Features:**
- View all available endpoints
- Test API calls directly
- See request/response schemas
- Try out endpoints with different parameters

### API Base URL

```
https://localhost:7081/api
```

All endpoints use this base URL when called from the frontend.

### Available Endpoints

#### Sponsorship Requests (`/api/SponsorshipRequests`)
Requestor-side operations for managing personal requests.

- `GET /` - Get my sponsorship requests
  - Returns: List of requests for current user
  - Auth: Authenticated user

- `POST /` - Create new sponsorship request
  - Body: CreateSponsorshipRequestCommand
  - Returns: Request ID
  - Auth: Authenticated user

- `GET /{id}` - Get request detail
  - Returns: Full request details
  - Auth: Authenticated user (owner)

- `PUT /{id}` - Update draft request
  - Body: UpdateSponsorshipRequestCommand
  - Returns: 204 No Content
  - Auth: Authenticated user (owner)

- `POST /{id}/submit` - Submit request
  - Returns: 204 No Content
  - Auth: Authenticated user (owner)

- `POST /{id}/cancel` - Cancel request
  - Body: CancelRequestCommand
  - Returns: 204 No Content
  - Auth: Authenticated user (owner)

#### Admin Requests (`/api/AdminRequests`)
System admin-only operations for monitoring all requests.

- `GET /` - Get all requests
  - Returns: List of all requests with pagination
  - Auth: SystemAdmin role required

- `GET /{id}` - Get request detail with history
  - Returns: Request with complete workflow history
  - Auth: SystemAdmin role required

#### Manager Approvals (`/api/ManagerApprovals`)
Manager-specific approval operations.

- `GET /` - Get pending manager approvals
  - Returns: List of requests waiting for manager approval
  - Auth: Manager role required

- `POST /{id}/approve` - Approve request
  - Body: ApproveRequestCommand
  - Returns: 204 No Content
  - Auth: Manager role required

- `POST /{id}/reject` - Reject request
  - Body: RejectRequestCommand
  - Returns: 204 No Content
  - Auth: Manager role required

#### Finance Approvals (`/api/FinanceApprovals`)
Finance admin-specific approval operations.

- `GET /` - Get pending finance approvals
  - Returns: List of requests waiting for finance approval
  - Auth: FinanceAdmin role required

- `POST /{id}/approve` - Approve request
  - Body: ApproveRequestCommand
  - Returns: 204 No Content
  - Auth: FinanceAdmin role required

- `POST /{id}/reject` - Reject request
  - Body: RejectRequestCommand
  - Returns: 204 No Content
  - Auth: FinanceAdmin role required

#### Sponsorship Types (`/api/SponsorshipTypes`)
Read-only endpoint for sponsorship type reference data.

- `GET /` - Get all sponsorship types
  - Returns: List of available sponsorship types
  - Auth: Authenticated users

### Authentication

All endpoints require JWT bearer token authentication. Include the token in request headers:

```
Authorization: Bearer <your-jwt-token>
```

**Token Flow:**
1. User logs in via identity endpoint
2. Backend returns JWT token
3. Angular stores token in local storage
4. All API requests include token in Authorization header
5. Backend validates token before processing request

---

## Troubleshooting

### Common Issues & Solutions

#### 1. PostgreSQL Connection Error

**Error Message:**
```
Failed to connect to postgres://localhost:5432
FATAL: database does not exist
```

**Solutions:**
1. Verify PostgreSQL is running:
   ```bash
   # Windows
   Get-Service postgresql*

   # Linux
   sudo systemctl status postgresql
   ```

2. Check connection string in `appsettings.json`:
   ```json
   "DefaultConnection": "Host=localhost;Port=5432;..."
   ```

3. Verify database exists:
   ```bash
   psql -U postgres -l | grep tech_assessment
   ```

4. Recreate database:
   ```bash
   psql -U postgres
   CREATE DATABASE tech_assessment;
   \q
   ```

#### 2. EF Core Migration Error

**Error Message:**
```
The database cannot be dropped because it is currently open by 1 other session(s).
```

**Solution:**
```bash
# Close all connections
psql -U postgres

# In psql terminal:
SELECT pg_terminate_backend(pg_stat_activity.pid) 
FROM pg_stat_activity 
WHERE pg_stat_activity.datname = 'tech_assessment' 
AND pid <> pg_backend_pid();

# Drop and recreate
DROP DATABASE tech_assessment;
CREATE DATABASE tech_assessment;

# Exit psql
\q

# Re-run migrations
cd src/Web
dotnet ef database update
```

#### 3. Angular Build Error: ERESOLVE

**Error Message:**
```
npm ERR! code ERESOLVE
npm ERR! ERESOLVE unable to resolve dependency tree
```

**Solution:**
```bash
cd src/Web/ClientApp

# Clear cache
npm cache clean --force

# Remove node_modules and lock file
rm -rf node_modules package-lock.json

# Use legacy peer dependencies flag
npm install --legacy-peer-deps

# Start development server
npm start
```

#### 4. Port Already in Use

**Error Message:**
```
Address 0.0.0.0:7081 already in use
```

**Solutions:**

For .NET backend (ports 5000, 7081):
```bash
# Find process using port (Windows)
netstat -ano | findstr :7081

# Kill process
taskkill /PID <PID> /F

# Or find and kill on Linux:
lsof -ti:7081 | xargs kill -9
```

For Angular frontend (port 4200):
```bash
cd src/Web/ClientApp

# Use different port
ng serve --port 4201

# Or kill existing process
# Windows
netstat -ano | findstr :4200
taskkill /PID <PID> /F

# Linux
lsof -ti:4200 | xargs kill -9
```

#### 5. CORS Error in Browser

**Error Message:**
```
Access to XMLHttpRequest at 'https://localhost:7081/api/...' from origin 
'http://localhost:4200' has been blocked by CORS policy
```

**Solution:**
Backend already has CORS enabled for development. If error persists:

1. Check `src/Web/Program.cs` contains:
   ```csharp
   app.UseCors(static builder => 
       builder.AllowAnyMethod()
           .AllowAnyHeader()
           .AllowAnyOrigin());
   ```

2. Verify frontend API base URL is correct in services
3. Clear browser cache and restart both services

#### 6. Login Not Working

**Error Messages:**
```
Invalid username or password
User account is locked
User not found
```

**Solutions:**
1. Verify test user exists:
   ```bash
   psql -U postgres -d tech_assessment
   SELECT UserName, Email FROM AspNetUsers;
   ```

2. Recreate database with seed data:
   ```bash
   cd src/Web
   dotnet ef database drop -f
   dotnet ef database update
   ```

3. Check user password format:
   - Minimum 8 characters
   - Must contain uppercase, lowercase, number, special character
   - Example: `Requestor@123`

4. Verify authentication is enabled in DependencyInjection:
   ```csharp
   builder.AddAuthentication(...).AddBearerToken(...);
   ```

#### 7. Swagger Shows No Endpoints

**Error:**
Swagger UI is blank or shows generic error.

**Solutions:**
1. Rebuild solution:
   ```bash
   dotnet build
   ```

2. Restart backend:
   ```bash
   cd src/Web
   dotnet run
   ```

3. Clear browser cache:
   - Press `Ctrl + Shift + Delete`
   - Clear all browsing data

4. Check backend logs:
   - Look for Swagger generation errors
   - Verify all endpoint classes implement `IEndpointGroup`

#### 8. TypeScript Client Not Generating

**Error Message:**
```
web-api-client.ts is missing or outdated
```

**Solution:**
```bash
cd src/Web/ClientApp

# Regenerate from Swagger
npm run generate-client

# If script doesn't exist, run nswag directly:
nswag run nswag.json

# Restart Angular
npm start
```

#### 9. Token Expiration Issues

**Error:**
401 Unauthorized after some time.

**Solutions:**
1. Login again - token has expired
2. Check token expiration in identity configuration
3. Implement token refresh logic (future enhancement)

#### 10. Database Locked/Permission Issues

**Error Message:**
```
permission denied for schema public
role "user" does not exist
```

**Solution:**
```bash
psql -U postgres -d tech_assessment

# Grant permissions to user
GRANT ALL PRIVILEGES ON SCHEMA public TO assessment_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO assessment_user;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO assessment_user;
```

### Viewing Logs

#### Backend Logs

Run backend with detailed output:
```bash
cd src/Web
dotnet run --verbosity detailed
```

Common log locations:
- Console output: Printed to terminal
- Event Log (Windows): Look for application errors
- Syslog (Linux): `/var/log/syslog`

#### Frontend Logs

1. Open browser DevTools: `F12`
2. Go to **Console** tab
3. Common issues:
   - Red errors: JavaScript/TypeScript errors
   - Yellow warnings: Deprecation warnings
   - Network errors: API call failures

4. Go to **Network** tab to debug API calls:
   - See all HTTP requests
   - Check status codes (200, 401, 500, etc.)
   - View request/response headers and body

#### Database Logs

Enable query logging:
```bash
psql -U postgres -d tech_assessment

-- Enable query logging
ALTER SYSTEM SET log_statement = 'all';

-- Apply changes
SELECT pg_reload_conf();

-- Check logs (on Linux):
tail -f /var/log/postgresql/postgresql.log
```

---

## Development Workflow

### Backend Development

**Workflow for making backend changes:**

1. **Make code changes** in Domain, Application, or Infrastructure layers

2. **Create EF Core migration** (if database schema changes):
   ```bash
   cd src/Web
   dotnet ef migrations add MigrationName
   ```

3. **Apply migrations**:
   ```bash
   dotnet ef database update
   ```

4. **Rebuild and test**:
   ```bash
   dotnet build
   dotnet run
   ```

5. **Test in Swagger UI**:
   - https://localhost:7081/swagger/ui
   - Try endpoints directly in UI

### Frontend Development

**Workflow for making frontend changes:**

1. **Make changes** to components, services, or models

2. **Check for errors** in DevTools Console (F12)

3. **Hot reload** happens automatically with `ng serve`

4. **Test in browser**:
   - http://localhost:4200
   - Check Network tab for API calls

5. **Generate updated client** (if backend API changes):
   ```bash
   cd src/Web/ClientApp
   npm run generate-client
   ```

### Full Stack Integration Testing

**End-to-end workflow:**

1. **Start backend**:
   ```bash
   cd src/Web
   dotnet run
   ```

2. **Start frontend**:
   ```bash
   cd src/Web/ClientApp
   npm start
   ```

3. **Test complete workflow**:
   - Navigate through UI
   - Create/update requests
   - Monitor Network tab for API calls
   - Check console for errors

4. **Common workflow test**:
   - Login as Requestor
   - Create and submit request
   - Logout and login as Manager
   - Approve request
   - Logout and login as Finance
   - Approve request
   - Login as Admin and verify completion

### Code Scaffolding

The template includes scaffolding for common patterns:

**Create new command**:
```bash
cd src/Application

dotnet new ca-usecase \
  --name MyNewCommand \
  --feature-name MyFeature \
  --usecase-type command \
  --return-type void
```

**Create new query**:
```bash
cd src/Application

dotnet new ca-usecase \
  -n GetMyData \
  -fn MyFeature \
  -ut query \
  -rt MyDataVm
```

---

## Code Styles & Formatting

The project includes **EditorConfig** support for consistent coding styles:

- File: `.editorconfig`
- Applies to all developers across different editors (VS, VS Code, etc.)
- Defines indentation, naming conventions, spacing, etc.

All code should follow the established patterns in the solution.

---

## Build

Run build command to compile the solution:

```bash
dotnet build

# With specific configuration
dotnet build -c Release

# Restore packages first
dotnet restore
dotnet build
```

Expected output:
```
Build succeeded
(Some number) Warning(s)
(Some number) Error(s)
```

---

## Additional Resources

- **[.NET 10 Documentation](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-10)**
- **[ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)**
- **[Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)**
- **[Angular Documentation](https://angular.io/docs)**
- **[MediatR Documentation](https://github.com/jbogard/MediatR)**
- **[NSwag Documentation](https://github.com/RicoSuter/NSwag)**
- **[Clean Architecture Explained](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)**

---

## Support

For issues or questions:

1. Check the [Troubleshooting](#troubleshooting) section
2. Review application logs (backend and frontend)
3. Create an issue with:
   - Exact error message
   - Steps to reproduce
   - Your environment (OS, .NET version, Node version)
   - Recent changes made

---

**Last Updated**: 2024  
**Framework**: .NET 10, Angular 18+  
**Database**: PostgreSQL 12+  
**Status**: Production Ready

To run the tests:
```bash
dotnet test
```

## Help
To learn more about the template go to the [project website](https://cleanarchitecture.jasontaylor.dev). Here you can find additional guidance, request new features, report a bug, and discuss the template with other users.