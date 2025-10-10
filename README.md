# CollabDude - Collaborative Project Announcement Platform

A modern microservices-based web application for creating and managing project collaboration announcements. Built with Vue.js frontend and .NET 8 backend services.

## Project Overview

CollabDude is a platform where users can post project announcements, find collaborators, and manage applications. The system uses a microservices architecture with separate services for user management and announcements.

## Technology Stack

### Frontend
- Vue.js 3 with Composition API
- Vue Router for navigation
- Pinia for state management
- Bootstrap 5 for UI components
- Axios for API communication
- Vue Toastification for notifications

### Backend
- .NET 8 Web API
- Entity Framework Core with PostgreSQL
- JWT Authentication
- AutoMapper for object mapping
- FluentValidation for input validation

## Architecture

The project follows a microservices architecture with two main services:

### UserService (Port 5001)
Handles user authentication, registration, and role management.

### AnnounceService (Port 5002)
Manages project announcements, applications, comments, categories, and notifications.

## Key Features

- User registration and authentication with JWT tokens
- Create and manage project announcements
- Browse and search announcements by category, type, and tags
- Apply to projects and manage applications
- Comment system with nested replies
- Real-time notifications
- Category and tag management
- User role-based access control (Admin/User)

## Project Structure

```
src/
├── client/collab-dude/          # Vue.js frontend application
│   ├── src/
│   │   ├── components/          # Reusable Vue components
│   │   ├── views/               # Page components
│   │   ├── stores/              # Pinia state management
│   │   ├── router/              # Vue Router configuration
│   │   └── main.js              # Application entry point
│
└── server/CollabDude/
    ├── UserService.API/         # User management service
    ├── UserService.Application/ # Business logic layer
    ├── UserService.Domain/      # Domain entities and interfaces
    ├── UserService.Infrastructure/ # Data access layer
    │
    ├── AnnounceService.API/     # Announcement service
    ├── AnnounceService.Application/
    ├── AnnounceService.Domain/
    └── AnnounceService.Infrastructure/
```

## Getting Started

### Prerequisites
- Node.js 16+ and npm
- .NET 8 SDK
- PostgreSQL 13+

### Frontend Setup
```bash
cd src/client/collab-dude
npm install
npm run serve
```

### Backend Setup

1. Update connection strings in `appsettings.json` for both services
2. Run migrations:
```bash
cd src/server/CollabDude/UserService.Infrastructure
dotnet ef database update

cd ../AnnounceService.Infrastructure
dotnet ef database update
```

3. Start services:
```bash
# Terminal 1 - UserService
cd src/server/CollabDude/UserService.API
dotnet run

# Terminal 2 - AnnounceService
cd src/server/CollabDude/AnnounceService.API
dotnet run
```

## Default Test Credentials

Admin Account:
- Email: admin@example.com
- Password: Password123!

User Account:
- Email: ahmet.kaya@example.com
- Password: Password123!

## API Endpoints

### UserService (http://localhost:5001/api)
- POST /auth/login - User login
- POST /users - User registration
- GET /users - Get all users (Admin only)
- GET /roles - Get all roles (Admin only)

### AnnounceService (http://localhost:5002/api)
- GET /announces - List announcements with filters
- POST /announces - Create announcement (Auth required)
- GET /announces/{id}/details - Get announcement details
- POST /applications - Submit application
- GET /categories/active - Get active categories
- POST /comments - Add comment
- GET /notifications - Get user notifications

## Domain Models

### User
- Email, Username, Password
- FirstName, LastName
- Role (Admin/User)
- IsActive status

### Announce
- Title, Description, Content
- Category and CollaborationType
- Status (Draft, Active, Closed, Expired)
- MaxParticipants and CurrentParticipants
- Location, RequiredSkills, ContactInfo
- ExpiryDate, Tags

### Application
- ApplicantUsername, Message
- Status (Pending, Approved, Rejected, Withdrawn)
- ReviewNote
