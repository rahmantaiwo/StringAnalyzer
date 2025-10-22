# StringAnalyzer.API

Lightweight ASP.NET Core Web API (net9.0) that analyzes and stores strings. Exposes REST endpoints and a Swagger UI for exploration.

## Key features
- .NET 9 minimal API-style Web API project
- SQLite persistence via `AppDbContext`
- Swagger / OpenAPI UI (Swashbuckle)
- Endpoints for analyzing, querying, filtering (including natural language), and deleting strings

## Prerequisites
- .NET 9 SDK installed
- Visual Studio 2022 (recommended) or the `dotnet` CLI
- (Optional) SQLite client to inspect the database file

## Project layout (relevant files)
- `Program.cs` — app startup, DI, EF Core registration, Swagger
- `Controllers/StringsController.cs` — API endpoints
- `Services/Services/StringAnalyzerService.cs` — business logic
- `Persistence/AppDbContext.cs` — EF Core DbContext
- `Properties/launchSettings.json` — launch profiles and ports

## Running the project

From Visual Studio
1. Select the desired launch profile from the run target dropdown (examples: __IIS Express__, `https`).
2. Press F5 (Debug) or Ctrl+F5 (Run without debugging).

From the command line
1. cd into the `StringAnalyzer.API` project folder.
2. dotnet run
3. By default Kestrel will choose a local port. Use the URLs printed in the console to open the app.

Notes about ports/profiles
- The `https` profile exposes `https://localhost:7085` and `http://localhost:5062`.
- The __IIS Express__ profile uses the SSL port `44361` (this explains earlier requests to `https://localhost:44361`).
- You can change these in `Properties/launchSettings.json`.

## Swagger / OpenAPI
- Swagger UI is enabled and reachable at `/swagger` (e.g. `https://localhost:7085/swagger` or the IIS Express URL).
- If you see a 500 when loading `/swagger/v1/swagger.json`, check the __Output__ window (__Show output from: ASP.NET Core Web Server__) — a common cause is duplicate route attributes producing a `Swashbuckle.AspNetCore.SwaggerGen.SwaggerGeneratorException`.
  - Example fix: remove duplicate `[HttpGet("test")]` attributes in `StringsController.cs` so each method/path combination is unique.

## API Endpoints (summary)
Base route: `strings`

- GET `/strings/test`  
  - Health/test endpoint. Returns a simple OK string.
- POST `/strings`  
  - Body: `CreateStringRequest` DTO — analyze and persist a string. Returns analysis result.
- GET `/strings/{value}`  
  - Get a specific analyzed string by its value.
- GET `/strings`  
  - Query to list all strings. Accepts `FilterQueryParams` via query string for paging/filtering.
- GET `/strings/filter-by-natural-language?query={q}`  
  - Filter stored strings using a natural language query.
- DELETE `/strings/{value}`  
  - Delete an analyzed string by value.

(See `Controllers/StringsController.cs` and DTOs in `Models/DTOs` for request/response shapes.)

## Database / Migrations
- SQLite is configured in `Program.cs` using the `DefaultConnection` connection string in `appsettings.json`.
- If you use EF Core migrations, run:
  - dotnet ef migrations add <Name> --project StringAnalyzer.API
  - dotnet ef database update --project StringAnalyzer.API

If no migrations are provided, the app may create the database on first run depending on the `AppDbContext` configuration.

## Troubleshooting
- Swagger 500 on `/swagger/v1/swagger.json`: check the exception in the __Output__ window — commonly caused by duplicate route decorations. Ensure each route + HTTP method combination is unique.
- If HTTPS certificate errors occur when using __IIS Express__, re-launch Visual Studio or reset the dev certificate (`dotnet dev-certs https --trust`).
- For runtime exceptions, inspect logs in the __Output__ window or run with console logging enabled (`dotnet run`).

## Tests & CI
- If tests exist, run them via Visual Studio Test Explorer or `dotnet test` at the solution level.

## Contributing
- Follow the existing coding style and register new services in `Program.cs`.
- Add controller routes under the `strings` route (or new controllers) and update Swagger by rebuilding.

## License
- Add your preferred license file at the repo root (`LICENSE`).

If you want, I can:
- Generate a repo-level `README.md` file in the project root.
- Create example `curl` or Postman requests for the endpoints.
- Inspect `StringAnalyzerService.cs` to add specific DTO examples to this README.