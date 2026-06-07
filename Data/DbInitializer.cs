using System;
using System.Linq;
using System.Threading.Tasks;
using HUTECH_Hospital.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace HUTECH_Hospital.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "Doctor", "Patient" };

            // Create roles if they don't exist
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create default admin user
            var adminEmail = "admin@hutech.edu.vn";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Quản trị viên HUTECH",
                    CreatedAt = DateTime.Now
                };

                var createPowerUser = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createPowerUser.Succeeded)
                {
                    // Assign Admin role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
