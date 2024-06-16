using DeliverySystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DeliverySystem.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!await roleManager.RoleExistsAsync("Administrator"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Administrator"));
                }

                if (!await roleManager.RoleExistsAsync("Courier"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Courier"));
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                var adminUser = new ApplicationUser
                {
                    UserName = "admin@fruitdelivery.com",
                    Email = "admin@fruitdelivery.com",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890"
                };

                if (userManager.Users.All(u => u.Email != adminUser.Email))
                {
                    await userManager.CreateAsync(adminUser, "Admin@123");
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                }

                var normalUser = new ApplicationUser
                {
                    UserName = "user@fruitdelivery.com",
                    Email = "user@fruitdelivery.com",
                    EmailConfirmed = true,
                    PhoneNumber = "0987654321"
                };

                if (userManager.Users.All(u => u.Email != normalUser.Email))
                {
                    await userManager.CreateAsync(normalUser, "User@123");
                    await userManager.AddToRoleAsync(normalUser, "User");
                }

                var courierUser = new ApplicationUser
                {
                    UserName = "courier@fruitdelivery.com",
                    Email = "courier@fruitdelivery.com",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567889"
                };

                var courierUser2 = new ApplicationUser
                {
                    UserName = "courier2@fruitdelivery.com",
                    Email = "courier2@fruitdelivery.com",
                    EmailConfirmed = true,
                    PhoneNumber = "123123123"
                };

                if (userManager.Users.All(u => u.Email != courierUser.Email))
                {
                    await userManager.CreateAsync(courierUser, "Courier@123");
                    await userManager.CreateAsync(courierUser2, "Courier@123");
                    await userManager.AddToRoleAsync(courierUser, "Courier");
                    await userManager.AddToRoleAsync(courierUser2, "Courier");
                }

                if (!context.Fruits.Any())
                {
                    context.Fruits.AddRange(
                        new Fruit
                        {
                            Name = "Apple",
                            Price = 3.20M,
                            Stock = 10000,
                            ImageUrl = "https://www.shutterstock.com/image-photo/red-apple-isolated-on-white-600nw-1727544364.jpg"
                        },
                        new Fruit
                        {
                            Name = "Banana",
                            Price = 3.50M,
                            Stock = 15000,
                            ImageUrl = "https://www.shutterstock.com/image-photo/bunch-bananas-isolated-on-white-600nw-1722111529.jpg"
                        },
                        new Fruit
                        {
                            Name = "Strawberry",
                            Price = 4.25M,
                            Stock = 5000,
                            ImageUrl = "https://c02.purpledshub.com/uploads/sites/41/2023/09/GettyImages_154514873.jpg?w=1029&webp=1"
                        }
                    );
                }

                await context.SaveChangesAsync();
            }
        }
    }

}
