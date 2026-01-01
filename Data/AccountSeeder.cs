using Microsoft.AspNetCore.Identity;
using QuanLySoTietKiem.Entity;

namespace QuanLySoTietKiem.Data
{
    public static class AccountSeeder
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Tạo các roles nếu chưa tồn tại
            await SeedRolesAsync(roleManager);

            // Tạo tài khoản admin
            await SeedAdminAccountAsync(userManager);

            // Tạo tài khoản user mẫu
            await SeedSampleUserAccountsAsync(userManager);
        }
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            //Kiem tra va tao role admin
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //Kiem tra va tao role user 
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
        private static async Task SeedAdminAccountAsync(UserManager<ApplicationUser> userManager)
        {
            // Kiểm tra xem admin đã tồn tại chưa
            var adminUser = await userManager.FindByEmailAsync("admin@example.com");

            if (adminUser == null)
            {
                // Tạo tài khoản admin mới
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@lehieunghia.io.vn",
                    EmailConfirmed = true,
                    FullName = "Quản trị viên",
                    CCCD = "075202123465",
                    PhoneNumber = "0399864429",
                    PhoneNumberConfirmed = true,
                    AvatarUrl = "https://www.google.com/url?sa=i&url=https%3A%2F%2Fwww.freepik.com%2Ffree-photos-vectors%2Fadmin-avatar&psig=AOvVaw1-SUr30tqVrYlSALYjUagv&ust=1763821329496000&source=images&cd=vfe&opi=89978449&ved=0CBUQjRxqFwoTCLiVltK4g5EDFQAAAAAdAAAAABAE",
                    Address = "Phường Thủ Đức, Thành phố Hồ Chí Minh",
                    SoDuTaiKhoan = 1000000000 // 1 tỷ VND
                };

                // Tạo tài khoản với mật khẩu
                var result = await userManager.CreateAsync(admin, "Admin@123456");

                if (result.Succeeded)
                {
                    // Gán role Admin cho tài khoản admin
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
        private static async Task SeedSampleUserAccountsAsync(UserManager<ApplicationUser> userManager)
        {
            // Tạo một số tài khoản người dùng mẫu
            string[] sampleEmails = new[]
            {
                "user1@lehieunghia.io.vn",
                "user2@lehieunghia.io.vn",
                "user3@lehieunghia.io.vn"
            };

            foreach (var email in sampleEmails)
            {
                // Kiểm tra xem user đã tồn tại chưa
                var existingUser = await userManager.FindByEmailAsync(email);

                if (existingUser == null)
                {
                    // Tạo tài khoản user mới
                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        FullName = $"Người dùng {email.Split('@')[0]}",
                        PhoneNumber = GenerateRandomPhoneNumber(),
                        CCCD = GenerateRandomCCCD(),
                        Address = "Phường Thủ Đức, Thành phố Hồ Chí Minh",
                        SoDuTaiKhoan = 1000000000 // 1 tỷ VND
                    };

                    // Tạo tài khoản với mật khẩu
                    var result = await userManager.CreateAsync(user, "User@123456");

                    if (result.Succeeded)
                    {
                        // Gán role User cho tài khoản user
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }
            }
        }
        // Hàm tạo số điện thoại 10 số ngẫu nhiên 
        private static string GenerateRandomPhoneNumber()
        {
            Random random = new Random();
            string phoneNumber = "0"; // Bắt đầu với số 0
            for (int i = 0; i < 9; i++)
            {
                phoneNumber += random.Next(0, 10).ToString();
            }
            return phoneNumber;
        }
        // Hàm tạo số CCCD ngẫu nhiên
        private static string GenerateRandomCCCD()
        {
            Random random = new Random();
            return random.Next(100000000, 999999999).ToString().PadRight(12, '0');
        }

    }
}