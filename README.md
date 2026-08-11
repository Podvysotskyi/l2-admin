# L2 Admin

Internal administration product for L2 live operations. It combines a Nuxt web interface with a read-only ASP.NET Core API for account and character directories.

## Architecture

The API is split into focused .NET projects:

- `server/src` — production projects
- `server/tests` — database-free server unit-test projects
- `L2.Admin.Api` — controllers, request filters, and HTTP endpoints
- `L2.Admin.Configurations` — service registration, CORS, service identity, and liveness
- `L2.Admin.Contracts` — models, requests, and responses
- `L2.Admin.Repositories.Interfaces` — repository abstractions
- `L2.Admin.Repositories` — SqlKata queries against the external Game Server PostgreSQL database
- `L2.Admin.Exceptions` — Admin-specific exceptions
- `*.Tests` — database-free unit tests for API, configuration, and repositories

The repository does not provision PostgreSQL and does not own migrations. Apply migrations from `l2-server` before using the directory endpoints. Admin repositories query the Server-owned `accounts` and `player` schemas without importing Server entities, `DbContext` classes, migrations, or domain services.

The Nuxt application follows the Nuxt 4 structure under `web/app`:

- `components/app` contains shared application-shell components.
- `components/pages` contains substantial page-specific sections.
- `pages` contains thin route components responsible for store and route orchestration.
- `services` contains calls to the Nuxt `/api` proxy.
- `stores` contains Pinia Setup Stores.
- `types` groups browser contracts into models, requests, and responses.
- `composables` and `utils` contain reusable behavior and pure helpers.

Web tests are organized under `web/test/unit`, `web/test/nuxt`, and `web/test/e2e`.

## Prerequisites

- Docker Engine with Docker Compose
- An external Game Server PostgreSQL database with current `l2-server` migrations

## Development

Start the isolated Admin stack from this repository:

```sh
docker compose up --build
```

Compose starts `admin-api` and `admin`. Only the Admin UI publishes a host port. Compose selects `APP_ENV=development`; the web image loads `web/.env.development`, which points the Nuxt proxy at `http://admin-api:8080` inside the Compose network.

The API container uses `server/src/L2.Admin.Api/appsettings.Development.json` and the .NET image default port `8080`. Override `ConnectionStrings__PostgreSql` when the external database differs from the checked-in development value. Its Compose health check calls `/health/live` and does not query PostgreSQL. The Admin stack remains intentionally separate from the root `l2-infra` Compose model.

The Nuxt server proxies all browser `/api` requests to the Admin API. Browser code never calls the upstream API directly.

Separate GitHub workflows validate the web application, API, and Compose model. Pull requests and `main` pushes only validate. The Compose workflow parses the model and builds both development images. Pushing a `v*` tag validates both applications and then publishes `ghcr.io/podvysotskyi/l2-admin` and `ghcr.io/podvysotskyi/l2-admin-api` with the Git tag and `latest` tags.

The published web image is built from `.env.production` and runs its compiled Nuxt server. The published API image starts `L2.Admin.Api.dll`, defaults to `ASPNETCORE_ENVIRONMENT=Production`, and loads `appsettings.Production.json`; deployment environment variables may override those values through standard ASP.NET Core configuration. Both runtime images run as non-root users.

## Checks

Run every test, type-check, build, publish validation, and Compose validation inside Docker from the repository root:

```sh
docker build --target validate --tag l2-admin-web-validate web
docker build --target validate --file server/Dockerfile --tag l2-admin-api-validate .
docker run --rm --volume "$PWD:/workspace" --workdir /workspace docker:29-cli compose config
docker compose build
```

The web `validate` target installs locked dependencies, runs Vitest, type-checks, and builds Nuxt. The API `validate` target restores dependencies, builds and publishes the solution, and runs all server tests. Do not run `npm test`, `npm run typecheck`, `npm run build`, `dotnet build`, `dotnet test`, or `dotnet publish` directly on the host.

## Codex skills

When this repository is checked out through `l2-infra`, use `$develop-l2-admin` for Nuxt, API, contract, and full-product work. The skill selects the relevant Docker `validate` targets and adds the Compose checks for changes that cross the browser/API boundary.

Do not commit production connection strings, administrator credentials, tokens, original game files, or generated private assets.

## License

L2 Admin is licensed under the [GNU Affero General Public License v3.0 only](LICENSE) (`AGPL-3.0-only`).
