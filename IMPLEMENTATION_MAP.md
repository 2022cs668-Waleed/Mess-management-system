# Implementation Map — Auth, Validation, Roles, AJAX, Responsive GUI

This document maps where key features are implemented in the project and what to change to extend them.

1) Authentication & Authorization
- Files:
  - `Program.cs` — Identity registration and cookie settings (`builder.Services.AddIdentity<...>()`, `ConfigureApplicationCookie`, `UseAuthentication()`, `UseAuthorization()`).
  - `Data/SeedData.cs` — seeds roles (`Admin`, `Student`) and initial users.
  - `Data/ApplicationDbContext.cs` — Identity DB context (`IdentityDbContext<ApplicationUser>`).
  - Controllers with role restrictions: e.g. `Controllers/AdminController.cs` uses `[Authorize(Roles = "Admin")]`.
  - Views check roles: `Views/Shared/_Layout.cshtml` and `Views/Dashboard/Index.cshtml` use `User.IsInRole(...)` to show/hide UI.

Notes:
- Current auth uses ASP.NET Identity with cookie-based authentication. No JWT bearer authentication is configured.

2) JWT Authentication (NOT currently implemented)
- Where to add:
  - Modify `Program.cs` to call `builder.Services.AddAuthentication(...).AddJwtBearer(...)` with token validation parameters.
  - Add an API endpoint to issue tokens (e.g. `Controllers/Api/AuthController.cs`) that validates credentials and returns a signed JWT.
  - Secure API endpoints with `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` or configure default scheme.

Quick pointers (what to implement):
- Install `Microsoft.AspNetCore.Authentication.JwtBearer`.
- Add signing key + token options (store secrets in environment variables).
- Example spots: `Program.cs` (service registration), new `Controllers/Api/AuthController.cs` (token issuance), any `Controllers/Api/*` for endpoints.

3) Client-side validation (implemented)
- Files:
  - `ViewModels/*.cs` (e.g. `ViewModels/AccountViewModels.cs`) — DataAnnotations (`[Required]`, `[EmailAddress]`, `[Compare]`, custom `[GmailAddress]`) define validation rules.
  - `wwwroot/lib/jquery-validation` and `wwwroot/lib/jquery-validation-unobtrusive` — provide client-side validation logic.
  - `_ValidationScriptsPartial` is included on forms (e.g. `Views/Account/Login.cshtml` uses `@{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }` or `<partial name="_ValidationScriptsPartial" />`).
  - `Views/Shared/_Layout.cshtml` loads jQuery + unobtrusive scripts (CDN + local fallback) so client validation runs.

How it works:
- DataAnnotations create `data-val-*` attributes in rendered HTML helpers (`asp-for`). `jquery.validate` + `jquery.validate.unobtrusive` parse these attributes and run client-side validation before submit.

4) Server-side validation (implemented)
- Files:
  - Controllers validate incoming models using `ModelState.IsValid` (e.g. `Controllers/AdminController.cs`, account controllers).
  - ViewModels (DataAnnotations) ensure server validation matches client validation.

How it works:
- On POST actions the controller checks `if (!ModelState.IsValid) return View(model);` and errors are displayed by Razor using `asp-validation-for` and `asp-validation-summary` in views.

5) Roles & Rights (implemented)
- Files:
  - Role creation: `Data/SeedData.cs`.
  - Securing controllers/actions: `[Authorize]` attributes (e.g. `Controllers/AdminController.cs`).
  - UI-level control with `User.IsInRole(...)` checks in views (`Views/Shared/_Layout.cshtml`, `Views/Dashboard/Index.cshtml`).

6) Responsive GUI (implemented)
- Files:
  - `Views/Shared/_Layout.cshtml` — uses Bootstrap (CDN) and FontAwesome.
  - `wwwroot/css/site.css` — project-specific styles.
  - Layout contains responsive navbar and sidebar markup; Bootstrap classes are used across views.

7) AJAX / API usage (partial / minimal)
- Current status:
  - The project is primarily server-rendered MVC. There are no dedicated `Controllers/Api/*` controllers discovered.
  - jQuery and `jquery.validate` use AJAX internally for unobtrusive `remote` validation where configured.
  - `wwwroot/js/site.js` (loaded in layout) is the place to implement fetch/ajax logic for partial updates and form submissions.

Where to add APIs and AJAX calls:
- Create API controllers under `Controllers/Api/` that return JSON (use `[ApiController]` + `[Route("api/[controller]")]`).
- Use fetch or `$.ajax` in `wwwroot/js/site.js` or per-view script blocks to call these endpoints and update the DOM without a full page reload.
- Protect API endpoints with JWT or cookie auth and role checks (`[Authorize]`).

8) Static assets and client scripts
- Files:
  - `wwwroot/css/site.css` — app styles
  - `wwwroot/js/site.js` — client JS (place to add AJAX/UI code)
  - `wwwroot/lib/...` — libraries (jQuery, Bootstrap, validation)
- Layout loads CDN resources with fallbacks so the UI still works if local libs are missing.

9) Seeding and DB schema notes
- `Data/SeedData.cs` seeds roles/users/menus. It assumes the schema exists; Program.cs handles DB creation/migration depending on provider.
- If you want to support Postgres migrations properly, generate migrations with Npgsql locally and commit migrations that target Postgres.

10) Recommended next steps to get full JWT + SPA-like behavior
- Add JWT support:
  - Update `Program.cs` to register JWT bearer authentication.
  - Implement `Controllers/Api/AuthController.cs` to issue tokens.
  - Secure API controllers with `[Authorize]` and roles.
- Convert key form actions to AJAX:
  - For example, convert attendance marking, menu CRUD, bill generation endpoints to JSON API.
  - In `wwwroot/js/site.js` implement fetch calls and update sections of the page (tables, alerts) on success without full reload.
- Keep unobtrusive validation for forms. For AJAX forms, validate client-side then send JSON; on server return validation errors as JSON and have client display them.

---

If you want, I can create this file in the repository (`IMPLEMENTATION_MAP.md`) so you can read it in the project root. Tell me to create it and I will add it now.

```csharp
var groups = ViewBag.MessGroups as IEnumerable<MessGroup>;
  if (groups != null) { foreach(var group in groups) { ... } }