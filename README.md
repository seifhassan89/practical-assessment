# Sanna Commerce Assessment

This workspace contains the following implementations:

## Assignment 1: Data Service Abstraction (C#)

- Project: `Data Service Task/DataService.Api`
- Implements `IDataService` to read lines from a text file.
- Adds abstraction layers via decorators:
  - Caching: `CachedDataServiceDecorator`
  - Logging: `LoggingDataServiceDecorator`

Run:

- `cd Data Service Task/DataService.Api`
- `dotnet run`

Or with Docker:

- `cd Data Service Task`
- `docker build -t dataservice-api .`
- `docker run --rm -p 8080:8080 dataservice-api`

## Assignment 2: SQL Upsert (single statement)

- Scripts:
  - `SQL Task/sql-tables-create.sql` — Creates and seeds the tables
  - `SQL Task/primary-solution-merge-query.sql` — Single-statement `MERGE` upsert
  - `SQL Task/alternative-solution-insert-update.sql` — Alternative `UPDATE` + `INSERT` approach

How to use:

1. Run `sql-tables-create.sql` to create and populate the tables.
2. Run either upsert script to synchronize `PurchasesSnapshot` with `Purchases`.

## Reusable Form Validation Hook (React + TypeScript)

- Project: `Reusable Form Validation Hook`
- Vite app demonstrating a reusable `useFormValidation` hook (validators + engine), login/register with mock auth and route guards, shared UI (Button, Card, FormField, ErrorSummary), accessible form patterns, Tailwind + SCSS, ESLint/Prettier, Husky + lint-staged + Commitlint.

Run:

- `cd Reusable Form Validation Hook`
- `npm install`
- `npm run dev`

Optional:

- `npm run quality` — lint, type-check, and production build

---

