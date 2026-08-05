# TicketSystem API

Full-stack ticket management system with AI-powered analysis, RAG document Q&A, and real-time notifications. Built with .NET 10 Clean Architecture.

> Frontend repo: [ticket-system-ui](https://github.com/nikolamilojkovic123/ticket-system-ui)

---

## Tech Stack

**Backend:** ASP.NET Core 10 · Clean Architecture · CQRS · FluentValidation  
**Auth:** Google OAuth 2.0 · Email/password (register, login, forgot/reset password) · JWT Bearer · Role-based (Admin)  
**Database:** SQL Server 2022 · Entity Framework Core 10  
**Messaging:** RabbitMQ (async event bus)  
**Caching:** Redis + FusionCache (hybrid L1/L2)  
**AI:** OpenAI GPT (analysis, chat, TTS) · Ollama llama3 (RAG) · nomic-embed-text (embeddings)  
**Real-time:** SignalR WebSocket notifications  
**Email:** MailKit SMTP  
**Testing:** xUnit · FluentAssertions  
**Deployment:** Docker Compose  

---

## Architecture

```
Blazor WASM (:4200)
       │  HTTP / WebSocket
       ▼
ASP.NET Core API (:5000)
       │
       ├── Controllers → SimpleMediator → Handlers (CQRS)
       │   Result<T> pattern · FluentValidation · JWT Auth
       │
       ├─► SQL Server    (EF Core)
       ├─► Redis         (FusionCache L2 + backplane)
       ├─► RabbitMQ      (async events)
       ├─► Ollama        (local LLM + embeddings)
       └─► SMTP          (email alerts)
```

### Project Structure

```
TicketSystem.API                     → Controllers, middleware, HTTP pipeline
TicketSystem.Application             → Commands, Queries, Handlers (CQRS)
TicketSystem.Domain                  → Entities, repository interfaces, domain logic
TicketSystem.Infrastructure          → OpenAI, Ollama, RabbitMQ, SignalR, MailKit
TicketSystem.Infrastructure.Database → EF Core DbContext, migrations, repositories
TicketSystem.Core                    → Result<T>, Error types
TicketSystem.Tests                   → Unit tests
```

---

## Features

### AI Ticket Analysis
- Automatic OpenAI analysis on ticket creation: summary, keywords, severity score (0-1)
- Vector embeddings via Ollama (`nomic-embed-text`) stored in SQL Server
- Hybrid search: semantic similarity (cosine) + keyword matching

### RAG Document Q&A
- Upload PDF/TXT documents → chunked and embedded via Ollama
- Questions answered by `llama3` using relevant chunks as context
- AI responses converted to speech via OpenAI TTS
- Answers can be posted as ticket comments

### Event-Driven Processing
- `TicketCreated` events published to RabbitMQ
- Background consumer triggers AI enrichment asynchronously
- Retry mechanism with dead-letter handling

### Real-Time Notifications
- SignalR WebSocket hub pushes events to connected users
- Ticket assigned, status changed, comment added
- Notification panel with unread count and mark-as-read

### Email Notifications
- Automatic emails: ticket creation (admin), assignment, status changes
- HTML templates via MailKit

### Auth
- Google OAuth 2.0 redirect flow → JWT Bearer tokens
- Email/password registration and login, with forgot/reset password via emailed token
- Role-based authorization (Admin panel)

### Dashboard & Admin
- Summary cards: active, resolved, AI suggestions, average severity
- Admin panel: per-user stats, resolution rate, average resolution time
- Admin user management: list all users, change user role

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/auth/google-login` | Initiate Google OAuth |
| GET | `/api/auth/google-response` | OAuth callback → JWT |
| POST | `/api/auth/register` | Register with email/password → JWT |
| POST | `/api/auth/login` | Login with email/password → JWT |
| POST | `/api/auth/forgot-password` | Request password reset email |
| POST | `/api/auth/reset-password` | Reset password with emailed token |
| GET | `/api/ticket` | List tickets (paginated, filterable) |
| POST | `/api/ticket` | Create ticket → AI enrichment + email |
| GET | `/api/ticket/{id}` | Get ticket by ID |
| PUT | `/api/ticket/{id}` | Update ticket |
| PUT | `/api/ticket/update-status/{id}` | Update ticket status |
| GET | `/api/ticket/search?q=` | Semantic + keyword search |
| POST | `/api/ticket/{id}/comments` | Add comment |
| GET | `/api/ticket/{id}/comments` | List comments |
| POST | `/api/documents/upload` | Upload + chunk + embed document |
| GET | `/api/documents` | List documents |
| POST | `/api/documents/{id}/ask` | RAG Q&A on stored document |
| POST | `/api/documents/ask` | Upload + ask in one request |
| POST | `/api/ai/chat` | Chat with OpenAI |
| GET | `/api/dashboard` | Dashboard stats |
| GET | `/api/user` | Current user profile |
| PUT | `/api/user` | Update current user profile |
| GET | `/api/user/all` | List users |
| GET | `/api/user/stats` | User statistics |
| GET | `/api/user/admin/all` | List all users (admin) |
| PUT | `/api/user/{id}/role` | Change a user's role (admin) |

---

## Running Locally

### Prerequisites
- Docker Desktop
- Google OAuth credentials
- OpenAI API key
- Ollama with `llama3` and `nomic-embed-text` models

### Setup

```bash
git clone https://github.com/nikolamilojkovic123/ticket-system-api.git
cd ticket-system-api
```

Create `.env`:
```env
MSSQL_SA_PASSWORD=YourPassword123!
GOOGLE_CLIENT_ID=your_google_client_id
GOOGLE_CLIENT_SECRET=your_google_client_secret
JWT_KEY=your-random-secret-key-minimum-32-characters
OPENAI_API_KEY=your_openai_api_key
UI_BASE_URL=http://localhost:4200
EMAIL_SENDER=your_email@gmail.com
EMAIL_PASSWORD=your_gmail_app_password
```

```bash
docker-compose up --build
```

| Service | URL |
|---------|-----|
| UI | http://localhost:4200 |
| API / Swagger | http://localhost:5000/swagger |
| RabbitMQ | http://localhost:15672 |

#### Alternative: infra in Docker, API/UI local

`run-local.ps1` starts only the infrastructure (SQL Server, Redis, RabbitMQ, Ollama) via `docker-compose.infra.yml`, then runs the API and UI directly with `dotnet run` in separate windows — useful for faster edit/debug cycles.

```powershell
.\run-local.ps1
```

| Service | URL |
|---------|-----|
| API / Swagger | http://localhost:5075/swagger |
| UI | http://localhost:5057 |

---

## Tests

```bash
dotnet test TicketSystem.Tests/TicketSystem.Tests.csproj
```

Covers: domain entities, Result pattern, document chunks, RAG service flow.
