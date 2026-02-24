# Campus Insider API

A modern ASP.NET Core 8 REST API powering a campus-focused social platform enabling students to share resources, coordinate rides, and communicate in real-time.

## Features

- **Authentication & Authorization** - JWT-based auth with role management
- **Post System** - Create offers/demands for materials, carpooling, media, tools
- **Real-time Chat** - SignalR-powered instant messaging
- **Real-time Notifications** - Push notifications via SignalR
- **Image Upload** - Automatic image processing and storage
- **Email Notifications** - SMTP-based email service
- **Statistics** - Platform usage analytics
- **API Documentation** - Swagger/OpenAPI integration
- **Rate Limiting** - Built-in protection against abuse

## Tech Stack

- **.NET 8** - Modern cross-platform framework
- **PostgreSQL** - Relational database
- **Entity Framework Core** - ORM with migrations
- **SignalR** - Real-time communication
- **JWT** - Token-based authentication
- **ImageSharp** - Image processing
- **MailKit** - Email functionality
- **Swashbuckle** - API documentation

## Prerequisites

- .NET 8 SDK
- PostgreSQL 14+
- Docker (optional)

## Configuration

Configure via `appsettings.json` or environment variables:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=campus-insider;Username=postgres;Password=your-password"
  },
  "Jwt": {
    "Key": "your-256-bit-secret-key",
    "Issuer": "https://your-domain.com",
    "Audience": "https://your-domain.com"
  },
  "Email": {
    "FromName": "Campus Insider",
    "FromAddress": "noreply@campus-insider.com",
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

## Getting Started

### 1. Clone and setup

```bash
cd campus-insider
dotnet restore
```

### 2. Configure database

Ensure PostgreSQL is running and update connection string in `appsettings.Development.json`.

### 3. Run migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

The API will be available at `http://localhost:5001`

### 5. API Documentation

Access Swagger UI at: `http://localhost:5001/swagger`

## Docker Deployment

```bash
docker build -t campus-insider .
docker run -p 5001:5001 campus-insider
```

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and get JWT |

### Posts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/post` | List posts (paginated) |
| POST | `/api/post` | Create new post |
| GET | `/api/post/{id}` | Get post details |
| PUT | `/api/post/{id}` | Update post |
| DELETE | `/api/post/{id}` | Delete post |

### Feed
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/feed` | Get personalized feed |

### Users
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user/me` | Get current user |
| PUT | `/api/user/me` | Update profile |

### Chat
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/chat` | Get conversations |
| POST | `/api/chat` | Send message |

### Notifications
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/notification` | Get notifications |

### Statistics
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/stats` | Get platform stats |

## Real-time Features

### SignalR Hubs

- `/hubs/chat` - Real-time messaging
- `/hubs/notifications` - Push notifications

### JavaScript Client Example

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("http://localhost:5001/hubs/notifications")
    .withAutomaticReconnect()
    .build();

await connection.start();
connection.on("ReceiveNotification", (message) => {
    console.log("New notification:", message);
});
```

## Project Structure

```
campus-insider/
├── Controllers/       # API controllers
├── Services/          # Business logic
├── Models/            # Entity models
├── DTOs/              # Data transfer objects
├── Data/              # DbContext and config
├── Hubs/              # SignalR hubs
├── Migrations/        # EF Core migrations
├── wwwroot/           # Static files
└── Program.cs         # Application entry point
```

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `PORT` | Server port | `5001` |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |

## License

MIT License
