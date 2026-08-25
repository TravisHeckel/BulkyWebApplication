// ---------------------------------------------------------------------------
// Program.cs is the application's ENTRY POINT and "composition root": it builds
// the web host, registers all services (dependency injection), wires up the HTTP
// request pipeline (middleware), and finally starts listening for requests.
// ---------------------------------------------------------------------------
using Bulky.DataAccess.Data;                       // ApplicationDbContext
using Bulky.DataAccess.Repository;                 // UnitOfWork
using Bulky.DataAccess.Repository.IRepository;     // IUnitOfWork
using Microsoft.EntityFrameworkCore;               // UseSqlServer
using Microsoft.AspNetCore.Identity;               // AddIdentity, IdentityUser/Role
using Microsoft.AspNetCore.Identity.UI.Services;   // IEmailSender
using Bulky.Utility;                               // EmailSender

var builder = WebApplication.CreateBuilder(args);

// ===== 1) SERVICE REGISTRATION (the DI container) =====
// Everything registered here can be requested by a constructor elsewhere (e.g. controllers).

// Enables MVC controllers + Razor views.
builder.Services.AddControllersWithViews();

// Register EF Core with SQL Server, reading the "DefaultConnection" string from
// appsettings.json. This is what makes ApplicationDbContext injectable everywhere.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Turn on ASP.NET Core Identity (registration/login/roles) backed by our DbContext.
// AddDefaultTokenProviders() enables tokens for email confirmation / password reset.
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// Tell Identity where the login/logout/access-denied pages live (the scaffolded Razor Pages).
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Identity's login/register UI is built with Razor Pages, so we must add them.
builder.Services.AddRazorPages();

// Register our data-access services. "Scoped" = one instance per HTTP request, which is
// exactly right for a DbContext/UnitOfWork. Controllers ask for IUnitOfWork and get UnitOfWork.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Supply the email sender Identity needs (this project uses the no-op stub).
builder.Services.AddScoped<IEmailSender,EmailSender>();

var app = builder.Build();

// ===== 2) HTTP REQUEST PIPELINE (middleware) =====
// Order matters here: each request flows top-to-bottom through this pipeline.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // In production, show a friendly error page and enable HSTS (force HTTPS in browsers).
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();   // redirect http:// -> https://
app.UseStaticFiles();        // serve wwwroot (css, js, images, uploaded product images)

app.UseRouting();            // decide which endpoint/controller handles the URL
app.UseAuthentication();     // WHO are you? (reads the login cookie)  -- must come before...
app.UseAuthorization();      // ...are you ALLOWED? ([Authorize] attributes)
app.MapRazorPages();         // map the Identity login/register Razor Pages

// The default MVC route. Note the {area=Customer} default: browsing "/" lands on
// Customer/Home/Index. "id?" means the id segment is optional.
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run(); // start the web server and block until shutdown
