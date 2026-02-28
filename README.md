# NZWalks

NZWalks is a full-stack project built with ASP.NET Core Web API and React (Vite + TypeScript) for managing NZ regions, walk difficulties, and walks.  
It includes role-based authorization, JWT + Refresh Token authentication, and a dashboard-style frontend for CRUD operations.

## Project Structure

- `api/NZWalks.API` - ASP.NET Core backend (JWT auth, role-based authorization)
- `frontend/nz-walks-ui` - React frontend (Tailwind CSS + Axios + CRUD dashboard)
- `conf/nginx` - Nginx reverse proxy config
- `docker-compose.yml` - Multi-container local deployment (nginx + api + mssql)
- `logs/` - Runtime logs for API, Nginx, and MSSQL

## Core Features

- API Versioning via URL segment (`/api/v1/...`)
- JWT Login/Register (`/api/v1/Auth/Login`, `/api/v1/Auth/Register`)
- Refresh token lifecycle:
  - Auto refresh access token and refresh token (`/api/v1/Auth/Refresh-Token`)
  - Revoke refresh token on logout (`/api/v1/Auth/Logout`)
- Role-based API access (`Reader`, `Writer`, `Admin`)
- CRUD UI for:
  - Regions
  - Difficulties
  - Walks
- Image upload (multipart/form-data) + static image hosting (`/Images/...`)
- Toast notifications and loading skeletons
- Token countdown and last-refresh time display in dashboard
- Route-based frontend pages:
  - Dashboard (`/`)
  - Register (`/register`)

## API Endpoint Summary

### Base URL

All controllers are versioned using a URL segment:

- Base path: `/api/v1`
- Controller route template: `/api/v{version}/[controller]` (current: `v1.0`)

### Auth

| Method | Endpoint | Auth Required | Purpose |
| --- | --- | --- | --- |
| POST | `/api/v1/Auth/Login` | No | Login and receive access + refresh token |
| POST | `/api/v1/Auth/Register` | No | Register new user (cannot include `Admin` role) |
| POST | `/api/v1/Auth/Refresh-Token` | No | Exchange refresh token for new token pair |
| POST | `/api/v1/Auth/Logout` | Yes (`Bearer`) | Revoke refresh token and logout |

### Regions

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| GET | `/api/v1/Regions` | `Reader`, `Admin` | Get all regions |
| GET | `/api/v1/Regions/{id}` | `Reader`, `Admin` | Get region by id |
| POST | `/api/v1/Regions` | `Writer`, `Admin` | Create region |
| PUT | `/api/v1/Regions/{id}` | `Writer`, `Admin` | Update region |
| DELETE | `/api/v1/Regions/{id}` | `Writer`, `Admin` | Delete region |

### Difficulties

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| GET | `/api/v1/Difficulties` | `Reader`, `Admin` | Get all difficulties |
| GET | `/api/v1/Difficulties/{id}` | `Reader`, `Admin` | Get difficulty by id |
| POST | `/api/v1/Difficulties` | `Writer`, `Admin` | Create difficulty |
| PUT | `/api/v1/Difficulties/{id}` | `Writer`, `Admin` | Update difficulty |
| DELETE | `/api/v1/Difficulties/{id}` | `Writer`, `Admin` | Delete difficulty |

### Walks

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| GET | `/api/v1/Walks` | No (AllowAnonymous) | Get walk list (supports filter/sort/page) |
| GET | `/api/v1/Walks/{id}` | No (AllowAnonymous) | Get walk by id |
| POST | `/api/v1/Walks` | `Writer`, `Admin` | Create walk |
| PUT | `/api/v1/Walks/{id}` | `Writer`, `Admin` | Update walk |
| DELETE | `/api/v1/Walks/{id}` | `Admin` | Delete walk |

Walk list query params:

- `filterOn` (string, optional)
- `filterQuery` (string, optional)
- `sortBy` (string, optional)
- `isAscending` (bool, optional; default `false`)
- `pageNumber` (int, optional; default `1`)
- `pageSize` (int, optional; default `1000`)

### Images

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| POST | `/api/v1/Image/Upload` | `Writer`, `Admin` | Upload an image (multipart/form-data) |
| GET | `/Images/{fileName}` | Public | Static file hosting (not a controller endpoint) |

Upload form fields:

- `File` (required, file)
- `FileName` (required, string)
- `FileDescription` (optional, string)

Upload constraints (enforced server-side):

- Allowed extensions: `.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp`, `.tiff`, `.ico`, `.webp`
- Max file size: 10MB

Static image hosting notes:

- `/Images/{fileName}` is served by **static files middleware** (configured in `api/NZWalks.API/Program.cs` with `RequestPath = "/Images"`), not by an API action.
- The URL will return `404` unless the file exists in the server's `Images/` folder at runtime.
- In Docker, `docker-compose.yml` mounts `./static/Images` into the API container, and Nginx also exposes it via `location /Images`.

## Local Development

### 1) Run Backend API

From `api/NZWalks.API`:

```bash
dotnet run --launch-profile https
```

Default local endpoints from `launchSettings.json`:

- `https://localhost:7238`
- `http://localhost:5017`

Swagger:

- `https://localhost:7238/swagger`
- (Direct API base) `https://localhost:7238/api/v1`

### 2) Run Frontend

From `frontend/nz-walks-ui`:

```bash
npm install
npm run dev
```

Open:

- `http://localhost:5173`

## Frontend API Configuration

Frontend Axios base URL (recommended):

- `https://localhost/api/v1`

This is configured in:

- `frontend/nz-walks-ui/src/services/apiClient.ts`

Also used by the auth client (`frontend/nz-walks-ui/src/services/authService.ts`).

If you are running the backend directly (without Nginx), set:

- `VITE_API_BASE_URL=https://localhost:7238/api/v1` (or `http://localhost:5017/api/v1`)

## Authentication Flow (Frontend)

The frontend implements token handling in `frontend/nz-walks-ui/src/services/authService.ts`:

1. Login stores both `jwtToken` and `refreshToken` in local storage.
2. A timer auto-refreshes tokens before JWT expiry.
3. If an API call returns `401`, Axios interceptor tries refresh and retries once.
4. Logout calls `/api/v1/Auth/Logout` to revoke refresh token, then clears local auth state.

Token request/response shapes (C# DTO names):

- Login/Refresh response: `{ "JWTToken": "...", "RefreshToken": "..." }`
- Refresh/Logout request body: `{ "JwtToken": "...", "RefreshToken": "..." }`

## Docker Deployment

Root `docker-compose.yml` defines:

- `reverse_proxy` (Nginx)
- `api` (ASP.NET Core)
- `mssql` (SQL Server)

Run:

```bash
docker compose up -d --build
```

Note: compose uses many values from env variables (`ENV_FILE_PATH`, ports, image tags, etc.). Ensure your env file is prepared before startup.

When running behind Nginx (reverse proxy):

- Frontend: `https://localhost/`
- API: `https://localhost/api/v1/...`
- Images: `https://localhost/Images/...`

## Authorization Notes

- `Regions` and `Difficulties` read endpoints require `Reader` or `Admin`.
- If dropdown options are empty in walk form, check:
  - user is logged in
  - JWT token exists in browser local storage
  - refresh token exists in browser local storage
  - account has correct roles

## Default Seeded Admin (Auth DB)

Auth DB seeds an `Admin` user and roles (`Reader`, `Writer`, `Admin`) via `NZWalksAuthDbContext`.
Default credentials are read from `api/NZWalks.API/appsettings.json`:

- Username: `admin`
- Email: `admin@example.com`
- Password: `Admin@123`

## Error Response Shape

Unhandled exceptions are returned as JSON by `ExceptionHandlerMiddleware`:

```json
{
  "Id": "guid",
  "ErrorMessage": "message",
  "StackTrace": "only in Development"
}
```

## Useful Paths

- Backend entry: `api/NZWalks.API/Program.cs`
- Frontend dashboard: `frontend/nz-walks-ui/src/pages/Dashboard.tsx`
- Frontend register page: `frontend/nz-walks-ui/src/pages/RegisterPage.tsx`
- Frontend auth service: `frontend/nz-walks-ui/src/services/authService.ts`
- Frontend API client/interceptor: `frontend/nz-walks-ui/src/services/apiClient.ts`
- Frontend token constants: `frontend/nz-walks-ui/src/constants/auth.ts`

## Showcase

### Dashboard

![Dashboard](./other/1.png)

### Register Page

![Register](./other/5.png)

### Region Management

![Region Management](./other/2.png)

### Difficulty Management

![Difficulty Management](./other/3.png)

### Walk Management

![Walk Management](./other/4.png)
