# Repository Guidelines

## Scope

This repository owns the internal L2 Admin product: the Nuxt web application, ASP.NET Core API, configuration composition, API contracts, repository interfaces and implementations, and Admin-specific exceptions. Login Server, Game Server, identity, character authority, database migrations, and PostgreSQL provisioning remain outside this repository.

The Admin service reads the external Game Server database. It does not own a database, migrations, or authoritative gameplay state.

Keep [docs/architecture.md](docs/architecture.md) aligned when these ownership or
data-flow boundaries change.

## Commands

Run development, every check, and every build through Docker from the repository root:

```sh
docker build --target validate --tag l2-admin-web-validate web
docker build --target validate --file server/Dockerfile --tag l2-admin-api-validate .
docker run --rm --volume "$PWD:/workspace" --workdir /workspace docker:29-cli compose config
docker compose build
```

Do not run development, checks, or builds with host-installed Node.js, npm, or .NET tooling.

Run the Admin-only development stack from this repository with `docker compose up --build`. The parent integration repository has no combined Compose model.

## Server Architecture

- Production projects live under `server/src`; server test projects live under `server/tests`. Keep shared build properties, package versions, the solution, and the Dockerfile at `server/`.
- `L2.Admin.Api` owns controllers, action-filter validation, and HTTP composition. Keep controllers thin.
- `L2.Admin.Configurations` owns dependency registration, CORS, service identity, and the process-level `/health/live` endpoint.
- `L2.Admin.Contracts` groups public DTOs by type under `Models`, `Requests`, and `Responses`.
- `L2.Admin.Repositories.Interfaces` owns repository abstractions.
- `L2.Admin.Repositories` owns SqlKata queries and internal database row contracts. Keep repository implementations at the project root and row contracts under `Contracts`.
- `L2.Admin.Exceptions` owns Admin-specific exception types.
- API, configuration, and repository test projects contain unit tests. Repository tests must not require a database connection.

Use SqlKata for all repository SQL construction. Do not import Server entities, `DbContext` types, migrations, or implementation projects. Every record, interface, and class belongs in its own `.cs` file.

## Web Architecture

Follow the Nuxt 4 directory structure under `web/app`. Pages own store wiring, route synchronization, loading, and composition. Reusable shell components live under `components/app`; substantial page sections live under `components/pages/<page>`.

All browser API calls go through the Nuxt `/api` proxy and the service layer. Do not call the Admin API directly from pages or components. Pinia stores use Setup Store syntax; expose state refs directly and reserve computed values for genuinely derived state. Avoid trivial setter actions.

Organize tests under `web/test/unit`, `web/test/nuxt`, and `web/test/e2e`. Keep pure state, service, and utility tests in `unit`.

## Configuration

`NUXT_ADMIN_API_BASE` is required whenever Nuxt configuration loads. Docker Compose selects the `development` target and `APP_ENV=development`; the published workflow selects the `production` target and `APP_ENV=production`. `APP_ENV` chooses the checked-in Nuxt build configuration, while `NODE_ENV` describes the running Node.js process.

Web and server workflows validate independently on pull requests and `main`. Only pushed `v*` tags publish either GHCR image; manual workflow runs never publish.

Environment-specific database configuration belongs in the matching `server/src/L2.Admin.Api/appsettings.<Environment>.json`. Environment variables may override it through standard ASP.NET Core configuration. Never commit administrator credentials, tokens, or original game files.

## Conventions

Use UTF-8 and LF endings. TypeScript and Vue use two-space indentation, single quotes, no semicolons, and no trailing commas. Preserve established C# formatting and nullable-reference-type safety.
