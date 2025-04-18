# QuizConnect

A web application for creating and taking quizzes.

## Project Structure

- **Server**: .NET 9.0 backend with MongoDB integration
  - RESTful API endpoints for user and admin functionality
  - Question management services
  - Authentication using tokens

- **Frontend**: React with TypeScript, Vite, and Tailwind CSS
  - User dashboard for taking quizzes
  - Admin dashboard for managing questions

## Getting Started

### Prerequisites

- Node.js
- .NET 9.0 SDK
- Docker and Docker Compose (for containerized deployment)
- MongoDB instance

### Development Setup

1. Clone the repository
2. Set up the backend:
   ```
   cd Server
   dotnet restore
   dotnet run
   ```

3. Set up the frontend:
   ```
   npm install
   npm run dev
   ```

### Deployment

Use Docker Compose for easy deployment:

```
cd Server
docker compose up -d
```

## API Documentation

See `Server/Server/apiDoc.md` for detailed API documentation.