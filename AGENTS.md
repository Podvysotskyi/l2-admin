# Repository Guidelines

## Scope

This repository owns only the Admin Nuxt frontend. Authorization, operational actions, audit records, and all persistence remain backend responsibilities.

## Commands

```sh
npm ci
npm test
npm run typecheck
npm run build
```

Use `NODE_AUTH_TOKEN` for private `@l2` package installation. Run `docker compose up --build` for containerized development.

## Conventions

Use UTF-8, LF endings, two-space indentation, single quotes, no semicolons, and no trailing commas. Keep the API endpoint configurable through `NUXT_PUBLIC_ADMIN_API_BASE`. Never place administrative credentials or tokens in frontend code.
