# L2 Admin Web

The L2 Admin product: a Nuxt web interface and a .NET Admin API for live operations, account support, and character administration.

## Prerequisites

- Node.js 22.13 or newer
- npm
- A GitHub Packages token with `read:packages` access, set as `NODE_AUTH_TOKEN`

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

The combined development model lives in the `l2-infra` repository. From its root, run PostgreSQL, the Admin API, and the Admin web application:

```sh
docker compose up --build admin-api admin
```

Do not commit original game files.

## Checks

```sh
cd web && npm test
cd web && npm run typecheck
cd web && npm run build
dotnet build server/L2.Admin.slnx
```

The Admin API uses the Admin-owned read model to query narrow account and character projections. It does not import Server entities, `DbContext` classes, migrations, or domain services.

## Dependencies

Admin consumes an explicit GitHub Packages release of `@l2/ui`. Update it deliberately and commit the resulting `package-lock.json`.
