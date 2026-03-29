namespace campus_insider.Data
{
    using campus_insider.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Security.Cryptography;
    using System.Text;


    public static class DbSeeder
        {
            public static async Task SeedAdminUser(WebApplication app)
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Check if admin already exists
                var adminEmail = "hanane@lsv77.fr";
                var adminExists = await context.Users.AnyAsync(u => u.Email == adminEmail);

                if (!adminExists)
                {
                    var admin = new User
                    {
                        Email = adminEmail,
                        // IMPORTANT: Hash this password using whatever logic your Login uses
                        Password = HashPassword("EpAPe)pm&Dj_3S="),
                        FirstName = "hanane",
                        LastName = "a",
                        Role = "ADMIN",
                        CreatedAt = DateTime.UtcNow,
                        IsEmailVerified = true,
                        EmailVerificationToken = null
                    };

                    context.Users.Add(admin);
                    await context.SaveChangesAsync();
                    Console.WriteLine("--> Admin account 'hanane@lsv77.fr' created successfully.");
                }


            }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var salt = GenerateSalt();
            var saltedPassword = password + salt;
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

            // Format: salt:hash
            return $"{salt}:{Convert.ToBase64String(hashBytes)}";
        }

        private static string GenerateSalt()
        {
            var saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

    }

}
