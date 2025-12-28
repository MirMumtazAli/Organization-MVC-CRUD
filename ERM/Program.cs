using Microsoft.EntityFrameworkCore;
using Organization.Data; // Enables Entity Framework Core features

// Create a WebApplication builder — entry point of your ASP.NET Core app
var builder = WebApplication.CreateBuilder(args);

// ✅ Add MVC controllers and views
// This line registers support for controllers and Razor views (MVC pattern)
builder.Services.AddControllersWithViews();

// ✅ Configure and register your EF Core DbContext
// Links your DbContext to SQL Server using the connection string from appsettings.json
builder.Services.AddDbContext<OrganizationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("constr"))
);

// Build the application — creates the app pipeline and dependencies
var app = builder.Build();

// ✅ Enable use of wwwroot folder for CSS, JS, images, etc.
app.UseStaticFiles();

// ✅ Enable request routing (maps URLs to controller actions)
app.UseRouting();

// ✅ Set up the default MVC route pattern
// This means if no specific route is given, it goes to:
// Controller: employees → Action: Index → optional parameter: id
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}"
);

// ✅ Run the web application
app.Run();
