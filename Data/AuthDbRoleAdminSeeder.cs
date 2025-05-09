using Microsoft.AspNetCore.Identity;

namespace HotelManagementSite.Data
{
    public static class AuthDbRoleAdminSeeder
    {
        public static async Task SeedAdminAndRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            var superAdminEmail = config["SuperAdmin:Email"];
            var superAdminPassword = config["SuperAdmin:Password"];
            var superAdminUserName = config["SuperAdmin:UserName"];


            var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (superAdminUser == null)
            {
                superAdminUser = new IdentityUser
                {
                    UserName = superAdminUserName,
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    PhoneNumber = "+8801774919639",
                    PhoneNumberConfirmed = true,
                };
                await userManager.CreateAsync(superAdminUser, superAdminPassword);
            }

            var roles = new List<string> { "SuperAdmin", "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

                if (!await userManager.IsInRoleAsync(superAdminUser, role))
                    await userManager.AddToRoleAsync(superAdminUser, role);
            }

        }

    }
}

