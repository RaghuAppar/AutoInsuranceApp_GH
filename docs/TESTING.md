# Testing Guide

## API (AutoInsuranceApi)

The API uses **NUnit** and **Moq** for unit tests. Tests run with the .NET CLI.

### Run API tests

```bash
cd AutoInsuranceApi.Tests
dotnet test
```

Or from the solution root:

```bash
dotnet test AutoInsuranceApi.Tests/AutoInsuranceApi.Tests.csproj
```

### What’s covered

- **AuthControllerTests** – Register/Login validation, success and failure (mocked `IAuthService`).
- **AuthServiceTests** – Registration and login with in-memory DB and mocked `IConfiguration`.
- **QuotesControllerTests** – Get all quotes, get by id, create quote with in-memory `AppDbContext` and mocked user claims.

### Dependencies

- NUnit, NUnit3TestAdapter  
- Moq  
- Microsoft.EntityFrameworkCore.InMemory  
- Microsoft.AspNetCore.Mvc.Testing, FrameworkReference Microsoft.AspNetCore.App  

---

## Client (AngularJS)

The client uses **Jasmine** (BDD) and **Karma** (test runner).

### Run client tests

```bash
cd Client
npm install
npm run test:once
```

Watch mode (re-run on file changes):

```bash
npm test
```

### Browser

By default Karma uses **ChromeHeadless**. If Chrome is not in the usual path, set:

- **Windows:** `set CHROME_BIN=C:\Path\To\chrome.exe`
- **Linux/macOS:** `export CHROME_BIN=/usr/bin/google-chrome`

To use Firefox instead, install `karma-firefox-launcher` and in `karma.conf.js` set `browsers: ['FirefoxHeadless']`.

### What’s covered

- **api.service.spec.js** – `apiService` get/post/put/delete (HTTP calls mocked with `$httpBackend`).
- **auth.service.spec.js** – `authService` getToken, getCurrentUser, setAuth, clearAuth, register, login, requireAuth.
- **quote.service.spec.js** – `quoteService` getAll, getById, create.
- **login.controller.spec.js** – `LoginController` initial state, validation, submit and redirect.
