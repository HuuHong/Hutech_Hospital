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
            
            // Cấp thêm quyền Admin cho lebao01121@gmail.com theo yêu cầu kiểm tra
            var lebaoUser = await userManager.FindByEmailAsync("lebao01121@gmail.com");
            if (lebaoUser != null)
            {
                if (!await userManager.IsInRoleAsync(lebaoUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(lebaoUser, "Admin");
                    await userManager.RemoveFromRoleAsync(lebaoUser, "Patient"); // Xóa role cũ nếu cần
                }
            }
            
            // Seed Departments
            if (!context.Departments.Any())
            {
                var departments = new[]
                {
                    new Department { Name = "Nội tổng quát", Description = "Khám và điều trị các bệnh lý nội khoa thông thường, theo dõi sức khỏe định kỳ.", ImageUrl = "https://images.unsplash.com/photo-1581594693702-fbdc51b2763b?w=400&q=80" },
                    new Department { Name = "Tim mạch", Description = "Chẩn đoán chuyên sâu, điện tâm đồ, siêu âm тим và điều trị bệnh lý tim mạch.", ImageUrl = "https://images.unsplash.com/photo-1628348068343-c6a848d2b6dd?w=400&q=80" },
                    new Department { Name = "Tai mũi họng", Description = "Khám và xử lý các tình trạng bệnh lý chuyên sâu liên quan đến Tai, Mũi, Họng.", ImageUrl = "https://images.unsplash.com/photo-1579684385127-1ef15d508118?w=400&q=80" },
                    new Department { Name = "Da liễu", Description = "Khám, tư vấn và điều trị các bệnh lý về da, tóc, móng và thẩm mỹ da.", ImageUrl = "https://images.unsplash.com/photo-1612349317150-e410f62d1101?w=400&q=80" },
                    new Department { Name = "Răng hàm mặt", Description = "Thẩm mỹ nha khoa, chỉnh nha niềng răng, nhổ răng không đau.", ImageUrl = "https://images.unsplash.com/photo-1606811841689-23dfddce3e95?w=400&q=80" },
                    new Department { Name = "Mắt", Description = "Kiểm tra thị lực, đo khúc xạ và điều trị các bệnh lý chuyên khoa mắt.", ImageUrl = "https://images.unsplash.com/photo-1512428559087-560fa5ceab42?w=400&q=80" },
                    new Department { Name = "Cơ xương khớp", Description = "Điều trị các chấn thương, bệnh lý xương khớp và vật lý trị liệu phục hồi chức năng.", ImageUrl = "https://images.unsplash.com/photo-1583324113626-70df0f4deaab?w=400&q=80" },
                    new Department { Name = "Tư vấn sức khỏe sinh viên", Description = "Phòng tư vấn tâm lý, đánh giá sức khỏe và hỗ trợ y tế đặc biệt cho sinh viên HUTECH.", ImageUrl = "https://images.unsplash.com/photo-1573497019940-1c28c88b4f3e?w=400&q=80" }
                };
                context.Departments.AddRange(departments);
                await context.SaveChangesAsync();
            }
            // Seed Medicines - Phase 4
            if (!context.Medicines.Any())
            {
                var medicines = new[]
                {
                    new Medicine { Name = "Paracetamol 500mg", Unit = "Viên", Description = "Giảm đau, hạ sốt", Usage = "1 viên x 2 lần/ngày sau ăn", IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Amoxicillin 500mg", Unit = "Viên", Description = "Kháng sinh đường hô hấp", Usage = "1 viên x 2 lần/ngày sau ăn", IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Vitamin C 500mg", Unit = "Viên", Description = "Tăng cường sức đề kháng", Usage = "1 viên/ngày sáng sau ăn", IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Omeprazole 20mg", Unit = "Viên", Description = "Thuốc trị viêm loét dạ dày", Usage = "1 viên trước ăn sáng 30 phút", IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Oresol", Unit = "Gói", Description = "Bù nước và điện giải", Usage = "Pha 1 gói với 200ml nước", IsActive = true, CreatedAt = DateTime.Now },
                    new Medicine { Name = "Berberin", Unit = "Viên", Description = "Thuốc đau bụng, tiêu chảy", Usage = "2 viên x 2 lần/ngày", IsActive = true, CreatedAt = DateTime.Now }
                };
                context.Medicines.AddRange(medicines);
                await context.SaveChangesAsync();
            }
        }
    }
}
