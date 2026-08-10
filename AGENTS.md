# Repository Guidelines

## Scope

This repository owns the Admin Nuxt frontend and the Admin API, Configurations, Contracts, Exceptions, Repositories.Interfaces, and Repositories projects. Server-owned identity, character, and content persistence remain outside this repository.

## Commands

```sh
cd web && npm ci
cd web && npm test
cd web && npm run typecheck
cd web && npm run build
dotnet build server/L2.Admin.slnx
dotnet test server/L2.Admin.slnx --no-build
```

Run the Admin-only development stack with this repository's `compose.yaml`; use the root `compose.yaml` in the `l2-infra` integration repository for the combined product stack.

## Conventions

Use UTF-8, LF endings, two-space indentation, single quotes, no semicolons, and no trailing commas. Keep the server-side API upstream configurable through `NUXT_ADMIN_API_BASE`. Never place administrative credentials or tokens in frontend code.
