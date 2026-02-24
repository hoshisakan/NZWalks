# NZ Walks UI

React + TypeScript frontend that integrates with `NZWalks.API`.

## Features

- Login to `POST /api/Auth/Login` and store JWT in local storage.
- Load public walks from `GET /api/Walks`.
- Load protected regions/difficulties from `GET /api/Regions` and `GET /api/Difficulties`.
- Editable API base URL in UI (default `/api`).

## Local Run

1. Start backend `NZWalks.API` first.
2. In this folder, run:

```bash
npm install
npm run dev
```

## API Proxy (Vite)

Dev server proxies `/api` to backend URL from `VITE_API_PROXY_TARGET`.

Create `.env.local` in this folder if you need custom backend address:

```bash
VITE_API_PROXY_TARGET=https://localhost:7238
```

If backend port changes, update this value and restart `npm run dev`.
