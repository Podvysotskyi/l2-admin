# L2 Admin Web

The L2 Admin product: a Nuxt web interface and a .NET Admin API for live operations, account support, and character administration.

## Prerequisites

- Node.js 22.13 or newer
- npm
- A GitHub Packages token with `read:packages` access, set as `NODE_AUTH_TOKEN`
- Admin API running at the configured endpoint

## Local development

```sh
export NODE_AUTH_TOKEN="$(gh auth token)"
cd web
npm ci
npm run dev
```

The Admin UI runs at <http://localhost:3002>. Override the backend endpoint when necessary:

```sh
NUXT_PUBLIC_ADMIN_API_BASE=http://localhost:5201 npm run dev
```

## Docker Compose

Run the repository-local Compose stack:

```sh
docker compose up --build
```

Do not commit original game files.

## Checks

```sh
cd web && npm test
cd web && npm run typecheck
cd web && npm run build
dotnet build server/L2.Admin.slnx
```

## Dependencies

Admin consumes an explicit GitHub Packages release of `@l2/ui`. Update it deliberately and commit the resulting `package-lock.json`.
