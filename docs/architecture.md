# L2 Admin architecture

L2 Admin is a read-only operational view over Server-owned account and
character data. It owns presentation and query contracts, but it never owns or
mutates authoritative player state.

## Boundaries

- `l2-server` owns accounts, sessions, characters, game versions, worlds, and
  every migration for those records.
- The Admin API connects to the external Server PostgreSQL database and builds
  read-only queries with SqlKata. It does not import Server entities, contexts,
  migrations, services, or implementation projects.
- Account queries are global. Character queries require a game-version key.
- A future production deployment may replace direct database reads with narrow
  operational endpoints or replicated projections without changing Admin's
  read-only authority.

## Application flow

The Nuxt application calls only its same-origin `/api` proxy. Browser services
own transport calls, Pinia stores own remote state, and pages coordinate routes
and presentation. The proxy calls the ASP.NET Core API, whose controllers map
Admin-owned contracts to repository queries.

Admin has no PostgreSQL service, migrations, background worker, or write API.
Its repository-owned Compose model runs only the web application and API and
expects a compatible external Server database.
