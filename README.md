# Marketplace API
 
A secure second-hand marketplace REST API built with ASP.NET Core. Users can register, create listings, and communicate with buyers and sellers through encrypted messaging.
 
## Features
 
- JWT authentication with optional Two-Factor Authentication (2FA)
- Encrypted messaging between buyers and sellers
- Argon2id password hashing

## About

This project is a combined exam project for the courses **Software Quality** and **Secure Software Development**. 

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
|--------|----------|-------------|------|
| POST | `/api/auth/Register` | Register a new user | No |
| POST | `/api/auth/Login` | Login and receive JWT | No |
| POST | `/api/auth/SetupTfa` | Enable 2FA and get QR code | JWT |
| POST | `/api/auth/ValidateTfa` | Validate TOTP code | 2FA JWT |
 
### Listings
 
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/listing/GetAll` | Get all listings | No |
| GET | `/api/listing/GetByUser` | Get listings by user | JWT |
| POST | `/api/listing/Create` | Create a listing | JWT |
| PUT | `/api/listing/Update` | Update a listing | JWT |
| DELETE | `/api/listing/Delete` | Delete a listing | JWT |
 
### Conversations
 
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/conversation/Create` | Start a conversation on a listing | JWT |
 
### Messages
 
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/message/send` | Send a message | JWT |
| GET | `/api/message/messages` | Get messages in a conversation | JWT |
 
### Users
 
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| PUT | `/api/user/Update` | Update own account | JWT |
| DELETE | `/api/user/Delete` | Delete own account | JWT |
 

## Postman Collection
 
Import `docs/Marketplace_https_postman_collection.json` to test the API flow.

Set the `baseUrl` variable to your API URL (e.g. `https://localhost:5001`) before running.

OBS!! Read docs inside postman collection to understand setup and recommended run order. 
 
