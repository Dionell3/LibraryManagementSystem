using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Apply pending migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Seed roles, default users and base data
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Roles
    foreach (var role in new[] { "Admin", "Librarian", "Member" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // Seed default users
    async Task<ApplicationUser> EnsureUser(string email, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, "Password123!");
        }
        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
        return user;
    }

    await EnsureUser("20032773@students.koi.edu.au", "Admin");
    await EnsureUser("20034750@students.koi.edu.au", "Librarian");

    // Seed a test member account
    var memberUser = await EnsureUser("member@library.com", "Member");
    if (!db.Members.Any(m => m.UserId == memberUser.Id))
    {
        db.Members.Add(new Member
        {
            MembershipNumber = "MEM001",
            FirstName = "Test",
            LastName = "Member",
            Email = "member@library.com",
            MemberSince = DateTime.Today,
            IsActive = true,
            UserId = memberUser.Id
        });
        await db.SaveChangesAsync();
    }

    // Seed default borrowing parameters
    if (!db.BorrowingParameters.Any())
    {
        db.BorrowingParameters.Add(new BorrowingParameters
        {
            Label = "Default",
            LoanDurationDays = 14,
            RenewalLimit = 2,
            OverduePenaltyPerDay = 0.50m,
            MaxBorrowableItems = 5
        });
        await db.SaveChangesAsync();
    }

    // Seed a default library
    if (!db.Libraries.Any())
    {
        db.Libraries.Add(new Library
        {
            Name = "KOI Main Library",
            Location = "Level 1, 545 Kent Street, Sydney NSW 2000",
            OperatingHours = "Mon-Fri: 8:00AM - 8:00PM, Sat: 9:00AM - 5:00PM",
            ContactDetails = "Phone: (02) 9283 9966 | Email: library@koi.edu.au"
        });
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
