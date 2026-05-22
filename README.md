# Marketplace API
 
A secure second-hand marketplace REST API built with ASP.NET Core. Users can register, create listings, and communicate with buyers and sellers through encrypted messaging.

## Features for Secure Software Development

- JWT authentication
- Optional Two-Factor Authentication (TOTP)
- Argon2id password hashing
- AES-GCM encrypted messaging
- File validation for uploaded listing images
- Authorization checks

## Features for Software Quality

- Cucumber tests
- Unit tests
- Postman collection for API testing

## About

This project is a combined exam project for the courses **Software Quality** and **Secure Software Development**.

The API allows users to:

- Register and authenticate using JWT
- Enable Two-Factor Authentication (2FA)
- Create and manage marketplace listings
- Upload listing images through Cloudinary
- Browse listings
- Start conversations between buyers and sellers
- Exchange AES-GCM encrypted messages inside conversations
- Manage their user profile

  
## Tech Stack
 
- **Backend:** C# / ASP.NET Core
- **Database:** PostgreSQL (Entity Framework Core)
- **Image storage:** Cloudinary

## Project Structure
 
```
/
├── server/
│   ├── Api.Rest/               # Controllers, middleware
│   ├── Application/            # Services, interfaces, DTOs, validators
│   ├── Core/                   # Domain entities
│   ├── Infrastructure/         # Postgres repos, scaffolding
|   ├── Specs/                  # Cucumber tests
│   └── UnitTests/              # xUnit tests
├── docs/
│   └── Marketplace_https_postman_collection.json
└── README.md
```
 
## Getting Started
 
### Prerequisites
 
- .NET 10 SDK
- PostgreSQL
- Cloudinary account

### Local Database (Docker)
 
A `docker-compose.yml` is included in the root of the project to setup up a local PostgreSQL container:
 
```bash
docker compose up -d
```
 
Then use the connection string in `appsettings.Development.json` to connect to the container:

```json
"AppOptions": {
    "DbConnectionString": "Server=localhost;Database=testdb;User Id=testuser;Password=testpass;"
  },
```
 
Once the container is running, apply the database schema by running the SQL script found in `Infrastructure.Postgres.Scaffolding/current_schema.sql`.

### Configuration
 
Add the following to `appsettings.json` or user secrets:
 
```json
{
  "AppOptions": {
    "JwtSecret": "<your-jwt-secret>"
  },
  "Encryption": {
    "Key": "<base64-encoded-32-byte-key>"
  },
  "TfaOptions": {
    "Issuer": "Marketplace",
    "Digits": 6,
    "Period": 30
  },
  "Cloudinary": {
    "CloudName": "<your-cloud-name>",
    "ApiKey": "<your-api-key>",
    "ApiSecret": "<your-api-secret>"
  }
}
```
 
## API Endpoints

### Auth

| Method | Endpoint | Description | Auth |
|----------|----------|-------------|------|
| POST | `/api/auth/Register` | Register a new user | No |
| POST | `/api/auth/Login` | Login and receive JWT | No |
| POST | `/api/auth/SetupTfa` | Enable 2FA and receive a QR code | JWT |
| POST | `/api/auth/ValidateOtp` | Validate TOTP code and complete login | 2FA JWT |

### Categories

| Method | Endpoint | Description | Auth |
|----------|----------|-------------|------|
| GET | `/api/categoryGetCategories` | Get all available categories | No |

### Listings

| Method | Endpoint | Description | Auth |
|----------|----------|-------------|------|
| GET | `/api/listing/GetAll` | Get all listings | No |
| GET | `/api/listing/GetListingById` | Get a listing by ID | No |
| GET | `/api/listing/GetAllByUserId` | Get listings owned by the authenticated user | JWT |
| POST | `/api/listing/Create` | Create a new listing with image uploads | JWT |
| PUT | `/api/listing/Update` | Update an existing listing | JWT |
| DELETE | `/api/listing/Delete` | Delete a listing | JWT |

### Conversations

| Method | Endpoint | Description | Auth |
|----------|----------|-------------|------|
| POST | `/api/conversation/Create` | Create or retrieve a conversation for a listing | JWT |
| GET | `/api/conversationGetOwnConversations` | Get all conversations for the authenticated user | JWT |

### Messages

| Method | Endpoint | Description | Auth |
|----------|----------|-------------|------|
| POST | `/api/message/send` | Send an AES-GCM encrypted message | JWT |
| GET | `/api/message/messages` | Get messages in a conversation | JWT |

### Users

| Method | Endpoint | Description | Auth |
|----------|----------|-------------|------|
| GET | `/api/user/GetUserByEmail` | Get the authenticated user's profile | JWT |
| PUT | `/api/user/Update` | Update own account | JWT |
| DELETE | `/api/user/Delete` | Delete own account | JWT |

## Postman Collection
 
Import `docs/Marketplace_https_postman_collection.json` to test the API flow.

Set the `baseUrl` variable to your API URL (e.g. `https://localhost:5001`) before running.

OBS!! Read docs inside postman collection to understand setup and recommended run order. 
 
