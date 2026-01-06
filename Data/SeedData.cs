using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using _2022_CS_668.Models;

namespace _2022_CS_668.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, 
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed Roles - Only Admin and Student
            string[] roleNames = { "Admin", "Student" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Mess Groups
            if (!await context.MessGroups.AnyAsync())
            {
                var messGroups = new List<MessGroup>
                {
                    new MessGroup
                    {
                        Name = "Water & Tea",
                        Description = "Mandatory group for all users - Water and Tea services",
                        IsMandatory = true,
                        IsActive = true
                    },
                    new MessGroup
                    {
                        Name = "Food",
                        Description = "Optional group - Full meal services",
                        IsMandatory = false,
                        IsActive = true
                    }
                };

                await context.MessGroups.AddRangeAsync(messGroups);
                await context.SaveChangesAsync();
            }

            // Seed Admin (with full system privileges)
            var adminEmail = "admin@gmail.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Students (Only 3 students)
            for (int i = 1; i <= 3; i++)
            {
                var studentEmail = $"student{i}@gmail.com";
                var student = await userManager.FindByEmailAsync(studentEmail);
                if (student == null)
                {
                    student = new ApplicationUser
                    {
                        UserName = studentEmail,
                        Email = studentEmail,
                        FullName = $"Student {i}",
                        EmailConfirmed = true,
                        PhoneNumber = $"123456789{i}",
                        IsActive = true
                    };

                    var result = await userManager.CreateAsync(student, "Student@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(student, "Student");

                        // Assign mandatory Water & Tea group
                        var waterTeaGroup = await context.MessGroups
                            .FirstOrDefaultAsync(m => m.Name == "Water & Tea");
                        
                        if (waterTeaGroup != null)
                        {
                            var userMessGroup = new UserMessGroup
                            {
                                UserId = student.Id,
                                MessGroupId = waterTeaGroup.Id,
                                IsActive = true
                            };
                            await context.UserMessGroups.AddAsync(userMessGroup);
                        }

                        // Assign some students to Food group as well
                        if (i % 2 == 0)
                        {
                            var foodGroup = await context.MessGroups
                                .FirstOrDefaultAsync(m => m.Name == "Food");
                            
                            if (foodGroup != null)
                            {
                                var userMessGroup = new UserMessGroup
                                {
                                    UserId = student.Id,
                                    MessGroupId = foodGroup.Id,
                                    IsActive = true
                                };
                                await context.UserMessGroups.AddAsync(userMessGroup);
                            }
                        }
                    }
                }
            }

            await context.SaveChangesAsync();

            // Seed Sample Menu Items
            if (!await context.Menus.AnyAsync())
            {
                var waterTeaGroup = await context.MessGroups
                    .FirstOrDefaultAsync(m => m.Name == "Water & Tea");
                var foodGroup = await context.MessGroups
                    .FirstOrDefaultAsync(m => m.Name == "Food");

                var menus = new List<Menu>();

                if (waterTeaGroup != null)
                {
                    // Add multiple water & tea items
                    menus.Add(new Menu
                    {
                        MessGroupId = waterTeaGroup.Id,
                        ItemName = "Morning Tea",
                        Category = MenuCategory.WaterTea,
                        Price = 10.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                    
                    menus.Add(new Menu
                    {
                        MessGroupId = waterTeaGroup.Id,
                        ItemName = "Evening Tea",
                        Category = MenuCategory.WaterTea,
                        Price = 10.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                    
                    menus.Add(new Menu
                    {
                        MessGroupId = waterTeaGroup.Id,
                        ItemName = "Drinking Water",
                        Category = MenuCategory.WaterTea,
                        Price = 5.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                }

                if (foodGroup != null)
                {
                    // Add multiple food items
                    menus.Add(new Menu
                    {
                        MessGroupId = foodGroup.Id,
                        ItemName = "Breakfast",
                        Category = MenuCategory.Food,
                        Price = 50.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                    
                    menus.Add(new Menu
                    {
                        MessGroupId = foodGroup.Id,
                        ItemName = "Lunch",
                        Category = MenuCategory.Food,
                        Price = 80.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                    
                    menus.Add(new Menu
                    {
                        MessGroupId = foodGroup.Id,
                        ItemName = "Dinner",
                        Category = MenuCategory.Food,
                        Price = 70.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                    
                    menus.Add(new Menu
                    {
                        MessGroupId = foodGroup.Id,
                        ItemName = "Snacks",
                        Category = MenuCategory.Food,
                        Price = 30.00m,
                        EffectiveDate = DateTime.Today.AddDays(-7),
                        IsActive = true
                    });
                }

                await context.Menus.AddRangeAsync(menus);
                await context.SaveChangesAsync();
            }
        }
    }
}
