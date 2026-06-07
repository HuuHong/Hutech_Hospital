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
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            else
            {
                // Nếu User đã tồn tại (có thể do đăng ký nhầm ngoài fontend), cần đảm bảo nó có role Admin
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            
            // Seed Departments
            if (!context.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { Name = "Nội tổng quát", Description = "Khám và điều trị các bệnh lý nội khoa thông thường.", ImageUrl = "https://images.unsplash.com/photo-1581594693702-fbdc51b2763b?w=400&q=80" },
                    new Department { Name = "Tim mạch", Description = "Chuẩn đoán chuyên sâu, điện tâm đồ, siêu âm тим.", ImageUrl = "https://images.unsplash.com/photo-1628348068343-c6a848d2b6dd?w=400&q=80" },
                    new Department { Name = "Tai mũi họng", Description = "Xử lý các tình trạng bệnh lý chuyên sâu TMH.", ImageUrl = "https://images.unsplash.com/photo-1579684385127-1ef15d508118?w=400&q=80" },
                    new Department { Name = "Nhi Khoa", Description = "Chăm sóc sức khỏe toàn diện cho trẻ sơ sinh và trẻ nhỏ.", ImageUrl = "https://images.unsplash.com/photo-1519494140681-8b17d7678b1d?w=400&q=80" },
                    new Department { Name = "Răng Hàm Mặt", Description = "Thẩm mỹ nha khoa, niềng răng, nhổ răng không đau.", ImageUrl = "https://images.unsplash.com/photo-1606811841689-23dfddce3e95?w=400&q=80" }
                };
                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }
        }
    }
}
