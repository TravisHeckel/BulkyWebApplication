# Bulky Web Application

A book-store web application built with **ASP.NET Core MVC (.NET 8)** and **Entity
Framework Core**, following an **N-tier (layered) architecture** with the
**Repository + Unit of Work** patterns. It supports product/category/company management
for admins, user accounts via ASP.NET Core Identity, and a per-user shopping cart with
tiered pricing.

This README explains how the project is put together and — importantly — **what the
ideal setup looks like from day one**, so the project stays healthy and buildable.

---

## 1. Solution layout (the "N-tier" architecture)

The solution is split into four projects. Each has one job, and they depend on each
other in **one direction only** (nothing lower depends on the web project):

```
BulkyWebApplication.sln
│
├── Bulky.Models         ← the data shapes (entities + view models). Depends on nothing app-specific.
├── Bulky.Utility        ← shared helpers/constants (role names, email sender stub).
├── Bulky.DataAccess     ← database layer: DbContext, migrations, repositories, unit of work.
└── BulkyWebApplication  ← the ASP.NET Core web app: controllers, views, Program.cs, wwwroot.
```

**Dependency flow:** `BulkyWebApplication` → `Bulky.DataAccess` → `Bulky.Models` → `Bulky.Utility`

Why split it up? Keeping data access out of the web project means the database logic is
reusable and testable, and the web layer only ever talks to clean interfaces
(`IUnitOfWork`), never to Entity Framework directly.

### How a request flows through the layers

```
Browser
  │  HTTP request  (e.g. GET /Admin/Category)
  ▼
Controller (BulkyWebApplication/Areas/.../Controllers)
  │  calls _unitOfWork.Category.GetAll()
  ▼
Unit of Work → Repository (Bulky.DataAccess/Repository)
  │  builds an EF Core query
  ▼
ApplicationDbContext (EF Core)  →  SQL Server database
  │  returns entities (Bulky.Models)
  ▼
Controller passes a Model/ViewModel to a View (.cshtml)
  ▼
Rendered HTML  →  Browser
```

---

## 2. Key building blocks (where to look)

| Concept | File | What it does |
|---|---|---|
| App startup / DI / middleware | `BulkyWebApplication/Program.cs` | Registers services, wires the request pipeline, starts the server. |
| Database context | `Bulky.DataAccess/Data/ApplicationDbContext.cs` | Maps entities to tables; seeds starter data. |
| Generic repository | `Bulky.DataAccess/Repository/Repository.cs` | Reusable `GetAll/Get/Add/Remove` for any entity. |
| Unit of Work | `Bulky.DataAccess/Repository/UnitOfWork.cs` | Bundles all repositories + a single `Save()`. |
| Entities | `Bulky.Models/*.cs` | `Category`, `Product`, `Company`, `ShoppingCart`, `ApplicationUser`. |
| View models | `Bulky.Models/ViewModels/*.cs` | Bundle several pieces of data for one screen. |
| Constants | `Bulky.Utility/SD.cs` | Central role-name constants ("Static Details"). |
| Areas | `BulkyWebApplication/Areas/{Admin,Customer,Identity}` | URL-grouped feature sections. |

**Areas** divide the app into sections that live under their own URL prefix:
- `Admin` – manage products, categories, companies.
- `Customer` – storefront, product details, shopping cart.
- `Identity` – login / register / account management (scaffolded by ASP.NET Core Identity).

> The `Areas/Identity/Pages/Account/**` files are **framework-generated** by the Identity
> scaffolder. They're intentionally left un-commented — they're boilerplate you normally
> don't edit. Everything *you* wrote (models, data access, controllers, `Program.cs`) is
> commented inline.

---

## 3. Prerequisites (the tools you need installed)

| Tool | Why | Notes |
|---|---|---|
| **.NET 8 SDK** | Build & run the app | `dotnet --version` should print `8.x`. |
| **Visual Studio 2022** (17.10+) or VS Code + C# Dev Kit | IDE | The `.sln` targets VS 2022. |
| **SQL Server** — LocalDB or Express | The database | LocalDB ships with Visual Studio and is the easiest option. |
| **EF Core tools** | Run migrations | `dotnet tool install --global dotnet-ef` (or use VS Package Manager Console). |

---

## 4. The ideal package setup (and the lesson behind it)

**The single most important rule for this project: keep every package version aligned
with the target framework.** All four projects target **`net8.0`**, so every Microsoft
package should be a **`8.0.x`** version. Mixing versions is exactly what broke this
project earlier.

### The healthy, consistent set for a .NET 8 build

| Package | Version | Which project(s) |
|---|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | `8.0.x` | Web, DataAccess, Models, Utility |
| `Microsoft.EntityFrameworkCore.Tools` | `8.0.x` | Web, DataAccess, Models, Utility |
| `Microsoft.EntityFrameworkCore.Design` | `8.0.x` | Web, DataAccess, Models, Utility |
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | `8.0.x` | Web, DataAccess |
| `Microsoft.AspNetCore.Identity.UI` | `8.0.x` | Web, Utility |
| `Microsoft.Extensions.Identity.Core` | `8.0.x` | Web |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | `8.0.x` | Web (only needed for scaffolding) |

### Three mistakes to avoid (these are what had broken the build)

1. **Don't mix major versions / previews.** The projects had EF Core **9.0.0-preview**
   packages mixed with Identity **8.0** packages on a **.NET 8** target. NuGet can't
   resolve that cleanly and restores fail or behave unpredictably. Pick one line (8.0.x)
   and use it everywhere.

2. **Don't add ASP.NET Core 2.2 packages to a .NET 8 web app.** Packages like
   `Microsoft.AspNetCore.Mvc.Core 2.2.5` and `Microsoft.AspNetCore.Mvc.ViewFeatures 2.2.0`
   were referenced. In a `Microsoft.NET.Sdk.Web` project **all of MVC is already provided
   by the shared framework** (`Microsoft.AspNetCore.App`). Adding the old standalone
   packages causes duplicate/ambiguous types and compatibility warnings. Rule of thumb:
   a Web SDK project needs **no** `Microsoft.AspNetCore.*` MVC packages at all.

3. **Don't leave orphaned provider packages.** A `Microsoft.EntityFrameworkCore.Sqlite`
   reference was left in the web project even though the app uses `UseSqlServer`. Unused
   references are confusing and invite version conflicts — remove what you don't use.

> These issues have already been corrected in the `.csproj` files. This section documents
> them so they don't creep back in.

### What a clean web `.csproj` looks like

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.10" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.10" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.10" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.10" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.10" />
    <PackageReference Include="Microsoft.Extensions.Identity.Core" Version="8.0.10" />
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.6" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Bulky.DataAccess\Bulky.DataAccess.csproj" />
    <ProjectReference Include="..\Bulky.Models\Bulky.Models.csproj" />
  </ItemGroup>
</Project>
```

---

## 5. Database configuration

The connection string lives in `BulkyWebApplication/appsettings.json` under
`ConnectionStrings:DefaultConnection` and is read in `Program.cs`.

**Recommended for a fresh start — use LocalDB** (not tied to a specific machine name):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Bulky;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

> The project currently points at `Server=TravisH\\SQLEXPRESS` — a specific PC's SQL Express
> instance. That only works on that machine. On any other machine (or after a reinstall)
> the app will build but fail at runtime with a database-connection error. Switching to
> `(localdb)\mssqllocaldb` avoids that.

---

## 6. First-time setup & running

```bash
# 1. Restore all NuGet packages
dotnet restore

# 2. Build the whole solution
dotnet build

# 3. Create/update the database from the EF Core migrations
#    (run from the folder that contains the .sln, pointing at the two projects)
dotnet ef database update \
  --project Bulky.DataAccess \
  --startup-project BulkyWebApplication

# 4. Run the web app
dotnet run --project BulkyWebApplication
```

Then open the HTTPS URL shown in the console (see
`BulkyWebApplication/Properties/launchSettings.json`, e.g. `https://localhost:7155`).

**In Visual Studio instead:** open `BulkyWebApplication.sln`, set `BulkyWebApplication`
as the startup project, use **Package Manager Console** →
`Update-Database` (with `Bulky.DataAccess` selected as the Default Project), then press **F5**.

### Migrations (EF Core)

Migrations live in `Bulky.DataAccess/Migrations/` and are **auto-generated** — don't edit
them by hand. To create a new one after changing an entity:

```bash
dotnet ef migrations add <DescriptiveName> \
  --project Bulky.DataAccess \
  --startup-project BulkyWebApplication
# then apply it with "dotnet ef database update" as above
```

---

## 7. Feature tour

- **Storefront** (`Customer/Home`): lists seeded books; product details page; add-to-cart.
- **Shopping cart** (`Customer/Cart`): per-user cart, +/- quantity, remove, live total with
  **tiered pricing** (1–50 / 51–100 / 100+ use `Price` / `Price50` / `Price100`).
- **Admin – Categories** (`Admin/Category`): classic server-rendered CRUD with confirm-delete.
- **Admin – Products** (`Admin/Product`): "Upsert" (create or edit) form with **image
  upload** to `wwwroot/images/product`, plus a JavaScript (DataTables) grid backed by JSON APIs.
- **Admin – Companies** (`Admin/Company`): Upsert + DataTables grid + AJAX delete.
- **Identity** (`Identity/Account`): register, login, logout, account management. Emails are
  handled by a **no-op `EmailSender`** stub (`Bulky.Utility/EmailSender.cs`) — swap in a real
  provider (SendGrid/SMTP) to actually send confirmation/reset messages.

Role constants for authorization live in `Bulky.Utility/SD.cs`. Several controllers have a
commented-out `[Authorize(Roles = SD.Role_Admin)]` you can enable to lock the admin area down.

---

## 8. Troubleshooting quick reference

| Symptom | Likely cause | Fix |
|---|---|---|
| NuGet restore errors / version conflicts | Mixed package versions (e.g. EF 9-preview + Identity 8) | Align every package to `8.0.x` (Section 4). |
| Hundreds of "ambiguous reference" build errors | ASP.NET Core 2.2 MVC packages in a .NET 8 web app | Remove `Microsoft.AspNetCore.Mvc.*` 2.2 packages. |
| Builds fine, crashes on a page that hits the DB | Connection string points at a SQL instance that isn't there | Use `(localdb)\mssqllocaldb` (Section 5) and run `database update`. |
| `dotnet ef` not found | EF tools not installed | `dotnet tool install --global dotnet-ef`. |
| Login/register pages 404 | Identity Razor Pages not mapped | Ensure `AddRazorPages()` + `app.MapRazorPages()` in `Program.cs` (they are). |
```
