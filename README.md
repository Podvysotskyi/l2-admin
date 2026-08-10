# L2 Admin Web

The Nuxt web interface for L2 live operations, account support, and character administration.

## Prerequisites

- Node.js 22.13 or newer
- npm
- A GitHub Packages token with `read:packages` access, set as `NODE_AUTH_TOKEN`
- Admin API running at the configured endpoint

## Local development

```sh
export NODE_AUTH_TOKEN="$(gh auth token)"
npm ci
npm run dev
```

The Admin UI runs at <http://localhost:3002>. Override the backend endpoint when necessary:

```sh
NUXT_PUBLIC_ADMIN_API_BASE=http://localhost:5201 npm run dev
```

## Docker Compose

```sh
export NODE_AUTH_TOKEN="$(gh auth token)"
docker compose up --build
```

Compose mounts any reviewed derived browser assets from the ignored `assets/` directory. Do not commit original game files.

## Checks

```sh
npm test
npm run typecheck
npm run build
```

## Dependencies

Admin consumes an explicit GitHub Packages release of `@l2/ui`. Update it deliberately and commit the resulting `package-lock.json`.
