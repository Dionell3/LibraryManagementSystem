using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Librarian"))
                await roleManager.CreateAsync(new IdentityRole("Librarian"));

            if (!await roleManager.RoleExistsAsync("Member"))
                await roleManager.CreateAsync(new IdentityRole("Member"));
        }
    }
}