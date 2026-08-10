# L2 Admin Web

The L2 Admin product: a Nuxt web interface and a .NET Admin API for live operations, account support, and character administration.

## Prerequisites

- Node.js 22.13 or newer
- npm

## Local development

```sh
cd web
npm ci
npm run dev
```

The Admin UI runs at <http://localhost:3002>. All browser API calls use the Nuxt `/api` proxy. Override the server-side upstream endpoint when necessary:

```sh
NUXT_ADMIN_API_BASE=http://localhost:5201 npm run dev
```

The web application follows the Nuxt 4 directory structure. Application code lives under `web/app`: API contracts in `types`, Nuxt-proxied requests in `services`, application state in `stores`, reusable view behavior in `composables`, and pure presentation/query helpers in `utils`. Static public files remain in `web/public`, and unit tests live in `web/test`.

## Docker Compose

Configure the external Game Server database and start the Admin API and web application from this repository:

```sh
cp .env.example .env
docker compose up --build
```

The Admin UI runs at <http://localhost:3002> and proxies API requests to the internal `admin-api` service. Only the Admin UI publishes a host port. Copy `.env.example` to `.env` and set `GAME_SERVER_DATABASE_CONNECTION_STRING` to the external Game Server PostgreSQL database.

The Admin repositories query the Server-owned `accounts` and `player` schemas. This repository does not provision a database or own migrations; apply the migrations from `l2-server` to the external database before using directory endpoints. The combined integration stack remains available from the `l2-infra` repository root.

Do not commit original game files.

## Checks

```sh
cd web && npm test
cd web && npm run typecheck
cd web && npm run build
dotnet build server/L2.Admin.slnx
dotnet test server/L2.Admin.slnx --no-build
```

The Admin API repository layer uses read-only SQL queries for narrow account and character projections. It does not import Server entities, `DbContext` classes, migrations, or domain services.
