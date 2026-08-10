# Repository Guidelines

## Scope

This repository owns the Admin Nuxt frontend and Admin API/read-model projects. Server-owned identity, character, and content persistence remain outside this repository.

## Commands

```sh
cd web && npm ci
cd web && npm test
cd web && npm run typecheck
cd web && npm run build
dotnet build server/L2.Admin.slnx
```

Use `NODE_AUTH_TOKEN` for private package installation. Container orchestration is defined by the repository-local `compose.yaml`.

## Conventions

Use UTF-8, LF endings, two-space indentation, single quotes, no semicolons, and no trailing commas. Keep the API endpoint configurable through `NUXT_PUBLIC_ADMIN_API_BASE`. Never place administrative credentials or tokens in frontend code.
