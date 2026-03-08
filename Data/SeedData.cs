using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketSystemProject.Models;

namespace TicketSystemProject.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            // DB hazır mı kontrol et
            await context.Database.MigrateAsync();

            // 1️⃣ Roller
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2️⃣ Admin
            var adminEmail = "admin@ticket.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = "Admin",
                    Email = adminEmail,
                    Name = "System Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // 3️⃣ Normal User
            var userEmail = "user@ticket.com";
            var normalUser = await userManager.FindByEmailAsync(userEmail);

            if (normalUser == null)
            {
                normalUser = new AppUser
                {
                    UserName = "User",
                    Email = userEmail,
                    Name = "Normal User",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(normalUser, "User123!");
                await userManager.AddToRoleAsync(normalUser, "User");
            }

            // 4️⃣ Ticket Seed (Eğer hiç ticket yoksa)
            if (!context.Tickets.Any())
            {
                var tickets = new List<Ticket>
                {
                    new Ticket
                    {
                        Title = "Login Issue",
                        Description = "User cannot login to the system.",
                        Status = "Open",
                        Priority = "High",
                        UserId = normalUser.Id
                    },
                    new Ticket
                    {
                        Title = "Payment Error",
                        Description = "Payment fails during checkout.",
                        Status = "Pending",
                        Priority = "Medium",
                        UserId = normalUser.Id
                    },
                    new Ticket
                    {
                        Title = "Dark Mode Request",
                        Description = "Please add dark mode support.",
                        Status = "Closed",
                        Priority = "Low",
                        UserId = adminUser.Id
                    }
                };

                context.Tickets.AddRange(tickets);
                await context.SaveChangesAsync();
            }
        }
    }
}