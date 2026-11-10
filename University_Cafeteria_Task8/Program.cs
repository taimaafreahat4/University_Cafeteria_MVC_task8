
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using University_Cafeteria_Task8.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Connection String
var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

// 2. Connect Identity to SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Identity and Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// 4. MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Error Handling
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// 6. HTTPS
app.UseHttpsRedirection();

// 7. Routing
app.UseRouting();

// 8. Authentication
app.UseAuthentication();

// 9. Authorization
app.UseAuthorization();

// 10. Static Files
app.MapStaticAssets();

// 11. MVC Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// 12. Identity Razor Pages
app.MapRazorPages()
   .WithStaticAssets();

// 13. Create Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "User", "Admin", "SuperAdmin" };

    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
        {
            var result = roleManager
                .CreateAsync(new IdentityRole(role))
                .GetAwaiter().GetResult();

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}

// First SuperAdmin setup - run once
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    // Replace this with the email of your intended administrator
    string adminEmail = "taimaafreahat310@gmail.com";

    var user = userManager.FindByEmailAsync(adminEmail)
        .GetAwaiter().GetResult();

    if (user != null)
    {
        bool isSuperAdmin = userManager
            .IsInRoleAsync(user, "SuperAdmin")
            .GetAwaiter().GetResult();

        if (!isSuperAdmin)
        {
            var result = userManager
                .AddToRoleAsync(user, "SuperAdmin")
                .GetAwaiter().GetResult();

            if (!result.Succeeded)
            {
                throw new Exception(
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }
    }
}

// 14. Run Application
app.Run();
