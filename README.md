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

- JWT Login/Register (`/api/Auth/Login`, `/api/Auth/Register`)
- Refresh token lifecycle:
  - Auto refresh access token and refresh token (`/api/Auth/Refresh-Token`)
  - Revoke refresh token on logout (`/api/Auth/Logout`)
- Role-based API access (`Reader`, `Writer`, `Admin`)
- CRUD UI for:
  - Regions
  - Difficulties
  - Walks
- Toast notifications and loading skeletons
- Token countdown and last-refresh time display in dashboard
- Route-based frontend pages:
  - Dashboard (`/`)
  - Register (`/register`)

## API Endpoint Summary

### Auth

| Method | Endpoint | Auth Required | Purpose |
| --- | --- | --- | --- |
| POST | `/api/Auth/Login` | No | Login and receive access + refresh token |
| POST | `/api/Auth/Register` | No | Register new user (`Reader`/`Writer`) |
| POST | `/api/Auth/Refresh-Token` | No | Exchange refresh token for new token pair |
| POST | `/api/Auth/Logout` | Yes (`Bearer`) | Revoke refresh token and logout |

### Regions

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| GET | `/api/Regions` | `Reader`, `Admin` | Get all regions |
| GET | `/api/Regions/{id}` | `Reader`, `Admin` | Get region by id |
| POST | `/api/Regions` | `Writer`, `Admin` | Create region |
| PUT | `/api/Regions/{id}` | `Writer`, `Admin` | Update region |
| DELETE | `/api/Regions/{id}` | `Writer`, `Admin` | Delete region |

### Difficulties

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| GET | `/api/Difficulties` | `Reader`, `Admin` | Get all difficulties |
| GET | `/api/Difficulties/{id}` | `Reader`, `Admin` | Get difficulty by id |
| POST | `/api/Difficulties` | `Writer`, `Admin` | Create difficulty |
| PUT | `/api/Difficulties/{id}` | `Writer`, `Admin` | Update difficulty |
| DELETE | `/api/Difficulties/{id}` | `Writer`, `Admin` | Delete difficulty |

### Walks

| Method | Endpoint | Roles | Purpose |
| --- | --- | --- | --- |
| GET | `/api/Walks` | No (AllowAnonymous) | Get walk list (supports filter/sort/page) |
| GET | `/api/Walks/{id}` | No (AllowAnonymous) | Get walk by id |
| POST | `/api/Walks` | `Writer`, `Admin` | Create walk |
| PUT | `/api/Walks/{id}` | `Writer`, `Admin` | Update walk |
| DELETE | `/api/Walks/{id}` | `Admin` | Delete walk |

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

### 2) Run Frontend

From `frontend/nz-walks-ui`:

```bash
npm install
npm run dev
```

Open:

- `http://localhost:5173`

## Frontend API Configuration

Frontend Axios default base URL:

- `https://localhost/api`

This is configured in:

- `frontend/nz-walks-ui/src/services/apiClient.ts`

If needed, you can override with `VITE_API_BASE_URL`.

## Authentication Flow (Frontend)

The frontend implements token handling in `frontend/nz-walks-ui/src/services/authService.ts`:

1. Login stores both `jwtToken` and `refreshToken` in local storage.
2. A timer auto-refreshes tokens before JWT expiry.
3. If an API call returns `401`, Axios interceptor tries refresh and retries once.
4. Logout calls `/api/Auth/Logout` to revoke refresh token, then clears local auth state.

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

## Authorization Notes

- `Regions` and `Difficulties` read endpoints require `Reader` or `Admin`.
- If dropdown options are empty in walk form, check:
  - user is logged in
  - JWT token exists in browser local storage
  - refresh token exists in browser local storage
  - account has correct roles

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
