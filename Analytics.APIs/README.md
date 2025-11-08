# Web Analytics Data Aggregator

A backend system for processing and aggregating web analytics data using .NET 8, RabbitMQ, and SQL Server.

Built as part of the ElectroPi Hiring Quest challenge.

---

## What Does It Do?

This system reads mock analytics data from JSON files (Google Analytics + PageSpeed Insights), processes them through a message queue, aggregates the data, and exposes reporting APIs.

**Key Features:**
- Real message broker (RabbitMQ) with retry logic and dead-letter queue
- Background consumer service for async processing
- Daily data aggregation with automatic updates
- JWT-secured REST APIs
- Clean architecture with separation of concerns

---

##  Tech Stack

- **.NET 8** - Web API & Background Services
- **SQL Server** - Data persistence
- **RabbitMQ** - Message broker
- **Entity Framework Core** - ORM (Code First)
- **JWT** - Authentication
- **BCrypt** - Password hashing
- **Swagger** - API documentation

---

##  Project Structure
```
Analytics/
├── Analytics.APIs/                    # API Controllers & Middleware
│   ├── Controllers/
│   ├── Middlewares/
│   └── MockData/                      # JSON mock files
├── Analytics.Core.Application/        # Business Logic & Services
├── Analytics.Core.Domain/             # Entities & Domain Interfaces
├── Analytics.Infrastructure/          # RabbitMQ & Background Worker
├── Analytics.Infrastructure.Persistence/  # EF Core & Repositories
└── Analytics.Shared/                  # DTOs & Settings
```

---

## Getting Started

### Prerequisites

Make sure you have these installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) (or SQL Server Express)
- [RabbitMQ](https://www.rabbitmq.com/download.html)

---

### Step 1: Clone the Repository
```bash
git clone https://github.com/mohameddsalah03/WebAnalytics
cd analytics-api
```

---

### Step 2: Configure Database

Open `Analytics.APIs/appsettings.json` and update the connection string:
```json
"ConnectionStrings": {
  "WebAnalyticsContext": "Server=.;Database=AnalyticsDB;Trusted_Connection=true;TrustServerCertificate=True"
}
```

> **Note:** Replace `Server=.` with your SQL Server instance name if needed.

---

### Step 3: Start RabbitMQ

**Option A: Windows Installer**
- Download and install from [rabbitmq.com](https://www.rabbitmq.com/download.html)
- RabbitMQ will start automatically as a Windows service

**Verify:** Open http://localhost:15672 (guest/guest)

---

### Step 4: Run Migrations
```bash
cd Analytics.APIs
dotnet ef database update
```

This creates the database and tables automatically.

---

### Step 5: Run the Application
```bash
dotnet run
```

The API will start at: **https://localhost:7011**

---

## Using the API

### 1. Open Swagger

Navigate to: **https://localhost:7011/swagger**

---

### 2. Register a User

**Endpoint:** `POST /api/Account/register`

**Request:**
```json
{
  "name": "ahmed",
  "email": "ahmed@example.com",
  "password": "Password!23"
}
```

**Response:**
```json
{
  "email": "ahmed@example.com",
  "token": "eyJhbGci..."
}
```

Copy the token from the response.

---

### 3. Authorize

1. Click the "Authorize" button at the top of Swagger
2. Enter: `Bearer space then <your-token>`
3. Click "Authorize"

---

### 4. Ingest Data

**Endpoint:** `POST /api/Ingestion/ingest`

Click **"Execute"** - this triggers the data ingestion process.

**What happens:**
1. System reads `MockData/ga_data.json` and `MockData/psi_data.json`
2. Combines them and publishes 10 messages to RabbitMQ
3. Background consumer processes and saves to database
4. Data is aggregated into daily statistics

---

### 5. View Reports

**Get Overview:**
```
GET /api/Reports/overview
```

**Response:**
```json
{
  "totalUsers": 859,
  "totalSessions": 1083,
  "totalViews": 2145,
  "avgPerformance": 0.888
}
```

**Get Per-Page Report:**
```
GET /api/Reports/pages
```

**Response:**
```json
[
  {
    "page": "/home",
    "totalUsers": 397,
    "totalSessions": 500,
    "totalViews": 1030,
    "avgPerformance": 0.89
  },
  ...
]
```

---


### Data Flow
```
JSON Files → DataIngestionService → RabbitMQ → Consumer Service → SQL Database → API Reports
```

1. **Producer:** Reads mock JSON files, combines GA + PSI data, publishes to RabbitMQ exchange
2. **RabbitMQ:** Distributes messages to queue
3. **Consumer:** Background service consumes messages with retry logic (3 attempts)
4. **Storage:** Saves raw data + creates daily aggregations
5. **APIs:** Returns aggregated reports

---

### Key Design Decisions

**Why RabbitMQ?**
- Decouples data ingestion from processing
- Built-in reliability with acknowledgments
- Dead-letter queue for failed messages

**Why Background Service?**
- Async processing doesn't block the API
- Can scale independently

**Why Unique Index on (Date, Page)?**
- Prevents duplicate data if ingestion runs multiple times
- Makes the system idempotent

**Why Retry Logic?**
- Handles transient failures (network issues, timeouts)
- Exponential backoff gives systems time to recover

---

## Database Schema
### Tables

--Users --
- Id (PK)
- Name
- Email (Unique)
- PasswordHash
- CreatedAt / UpdatedAt

-- RawAnalyticsData --
- Id (PK)
- Date, Page (Composite Unique Index)
- Users, Sessions, Views
- PerformanceScore, LcpMs
- CreatedAt / UpdatedAt

-- DailyStatistics --
- Id (PK)
- Date (Unique)
- TotalUsers, TotalSessions, TotalViews
- AvgPerformance
- CreatedAt / UpdatedAt

---

## Testing

### Manual Testing

1. Start RabbitMQ
2. Run the API: `dotnet run`
3. Use Swagger to test endpoints
4. Check RabbitMQ Management UI: http://localhost:15672
5. Verify data in SQL Server Management Studio

### Check Retry Logic

To see the retry mechanism in action:
1. Stop SQL Server temporarily
2. Trigger ingestion
3. Check console logs - you'll see 3 retry attempts with delays
4. Failed messages go to Dead Letter Queue (`analytics.dlq`)

---

## Troubleshooting

**RabbitMQ connection failed**
- Make sure RabbitMQ is running: `rabbitmq-service status` (Windows)
- Check connection settings in `appsettings.json`

**Database connection failed**
- Verify SQL Server is running
- Check the connection string in `appsettings.json`
- Make sure the database was created: `dotnet ef database update`

**Port already in use**
- Change the port in `launchSettings.json` under `applicationUrl`

---

## Project Dependencies

Key NuGet packages:
- `Microsoft.EntityFrameworkCore.SqlServer`
- `RabbitMQ.Client`
- `BCrypt.Net-Next`
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `Swashbuckle.AspNetCore`

---

## Security Notes

- Passwords are hashed using BCrypt before storage
- JWTs expire after 10 hours (configurable in `appsettings.json`)
- API endpoints are protected with `[Authorize]` attribute
- Connection strings should be moved to User Secrets for production
