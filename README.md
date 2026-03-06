# Auto Insurance Application

ASP.NET Core 8 Web API with SQLite backend and AngularJS 1.x client, based on the [Requirements](Requirements.md).

## Solution structure

- **AutoInsuranceApi** – .NET 8 Web API, REST endpoints, SQLite, JWT auth
- **Client** – AngularJS 1.x SPA (HTML, CSS, JS), user-friendly UI with full navigation

## REST API (ASP.NET Core 8)

Base URL: `http://localhost:5000/api`

| Area | Endpoints |
|------|-----------|
| **Auth** | `POST /auth/register`, `POST /auth/login` |
| **Profiles** | `GET /profiles/me`, `PUT /profiles/me` |
| **Vehicles** | `GET /vehicles`, `GET /vehicles/{id}`, `POST /vehicles`, `PUT /vehicles/{id}`, `DELETE /vehicles/{id}` |
| **Drivers** | `GET /drivers`, `GET /drivers/{id}`, `POST /drivers`, `PUT /drivers/{id}`, `DELETE /drivers/{id}` |
| **Quotes** | `GET /quotes`, `GET /quotes/{id}`, `POST /quotes` |
| **Policies** | `GET /policies`, `GET /policies/{id}`, `POST /policies/purchase`, `POST /policies/{id}/cancel` |
| **Claims** | `GET /claims`, `GET /claims/{id}`, `POST /claims`, `PUT /claims/{id}/status` |
| **Payments** | `GET /payments`, `GET /payments/{id}`, `POST /payments` |

All endpoints except `/auth/*` require `Authorization: Bearer <token>`.

Database: SQLite file `autoinsurance.db` (created in API project directory on first run).

## Run the API

```bash
cd AutoInsuranceApi
dotnet run
```

Runs at **http://localhost:5000** (and **https://localhost:5001** when using the HTTPS profile). The Client is served from the same origin at **http://localhost:5000** when you open that URL in the browser.

### HTTPS

To run with HTTPS (recommended for production-like testing):

1. Trust the dev certificate once (if not already):
   ```bash
   dotnet dev-certs https --trust
   ```
2. Run with the **https** profile so the app listens on both HTTP and HTTPS:
   ```bash
   dotnet run --launch-profile https
   ```
   Or in Visual Studio / Rider, choose the **https** launch profile.

- **HTTP:** http://localhost:5000  
- **HTTPS:** https://localhost:5001  

### Swagger (OpenAPI)

In **Development**, Swagger UI and the OpenAPI JSON are enabled:

- **Swagger UI:** http://localhost:5000/swagger (or https://localhost:5001/swagger when using HTTPS)
- **OpenAPI JSON:** http://localhost:5000/swagger/v1/swagger.json  

Use Swagger UI to try the API (e.g. `/api/auth/login`, `/api/auth/register`). For protected endpoints, call `/api/auth/login` first, copy the `token` from the response, then in Swagger click **Authorize** and enter `Bearer <your-token>`.

## Run the Client (AngularJS 1.x)

Serve the `Client` folder with any static server. Examples:

**Option A – Live Server (VS Code / npm)**

```bash
cd Client
npx live-server --port=8080 --open=/index.html
```

**Option B – Python**

```bash
cd Client
python -m http.server 8080
```

Then open **http://localhost:8080**. Log in or register; the app will call the API at `http://localhost:5000/api`.

**CORS:** The API allows origins `http://localhost:8080` and `http://127.0.0.1:8080` (and 5500). If you use another port, add it in `AutoInsuranceApi/Program.cs` in the `AddCors` block.

## Client navigation

- **Dashboard** – Summary of policies, quotes, claims
- **My Profile** – Address, license, DOB
- **Vehicles** – Add / edit / remove vehicles
- **Drivers** – Add / edit / remove drivers
- **Get a Quote** – Select vehicles and options, get premium
- **My Quotes** – List and open quote details, purchase policy
- **Policies** – List policies, view details, cancel
- **Claims** – List claims, file new claim, view claim details
- **Payments** – Payment history

UI is responsive and uses a single top navigation bar for all sections.

## Tech stack

- **API:** .NET 8, ASP.NET Core, EF Core, SQLite, JWT (Bearer), BCrypt
- **Client:** AngularJS 1.8, ngRoute, vanilla CSS
