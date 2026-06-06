using BookStore.Models;
using BookStore.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data
{
    public static class SeedData
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            using var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            const string adminRole = "Admin";

            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            const string adminEmail = "admin@bookstore.ru";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Админ",
                    LastName = "BookStore"
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Фантастика" },
                    new Category { Name = "Классика" },
                    new Category { Name = "Детективы" },
                    new Category { Name = "Романтика" }
                );
                await context.SaveChangesAsync();
            }

            // Авторы
            if (!context.Authors.Any())
            {
                context.Authors.AddRange(
                    new Author { FullName = "Джордж Оруэлл" },
                    new Author { FullName = "Айзек Азимов" },
                    new Author { FullName = "Лев Толстой" },
                    new Author { FullName = "Агата Кристи" },
                    new Author { FullName = "Джейн Остин" }
                );
                await context.SaveChangesAsync();
            }

            // Издательства
            if (!context.Publishers.Any())
            {
                context.Publishers.AddRange(
                    new Publisher { Name = "Эксмо" },
                    new Publisher { Name = "АСТ" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Books.Any())
            {
                var fantasy = context.Categories.First(c => c.Name == "Фантастика");
                var classic = context.Categories.First(c => c.Name == "Классика");
                var detective = context.Categories.First(c => c.Name == "Детективы");
                var romance = context.Categories.First(c => c.Name == "Романтика");

                var orwell = context.Authors.First(a => a.FullName.Contains("Оруэлл"));
                var asimov = context.Authors.First(a => a.FullName.Contains("Азимов"));
                var tolstoy = context.Authors.First(a => a.FullName.Contains("Толстой"));
                var christie = context.Authors.First(a => a.FullName.Contains("Кристи"));
                var austen = context.Authors.First(a => a.FullName.Contains("Остин"));

                var exmo = context.Publishers.First(p => p.Name == "Эксмо");

                context.Books.AddRange(
                    new Book
                    {
                        Title = "1984",
                        Description = "Антиутопия о тоталитарном обществе",
                        Price = 450,
                        Stock = 15,
                        AuthorId = orwell.Id,
                        CategoryId = fantasy.Id,
                        PublisherId = exmo.Id,
                        ImageUrl = "/images/books/1984.jpg"
                    },
                    new Book
                    {
                        Title = "Я, робот",
                        Description = "Классический сборник рассказов о роботах",
                        Price = 480,
                        Stock = 12,
                        AuthorId = asimov.Id,
                        CategoryId = fantasy.Id,
                        PublisherId = exmo.Id,
                        ImageUrl = "/images/books/i_robot.jpg"
                    },
                    new Book
                    {
                        Title = "Война и мир",
                        Description = "Эпический роман о войне и мире",
                        Price = 890,
                        Stock = 5,
                        AuthorId = tolstoy.Id,
                        CategoryId = classic.Id,
                        PublisherId = exmo.Id,
                        ImageUrl = "/images/books/war_and_peace.jpg"
                    },
                    new Book
                    {
                        Title = "Убийство в Восточном экспрессе",
                        Description = "Знаменитый детектив",
                        Price = 380,
                        Stock = 12,
                        AuthorId = christie.Id,
                        CategoryId = detective.Id,
                        PublisherId = exmo.Id,
                        ImageUrl = "/images/books/murder_on_the_orient_express.jpg"
                    },
                    new Book
                    {
                        Title = "Гордость и предубеждение",
                        Description = "Роман о любви и социуме",
                        Price = 420,
                        Stock = 10,
                        AuthorId = austen.Id,
                        CategoryId = romance.Id,
                        PublisherId = exmo.Id,
                        ImageUrl = "/images/books/pride_and_prejudice.jpg"
                    },
                    new Book
                    {
                        Title = "Скотный двор",
                        Description = "Сатирическая повесть",
                        Price = 320,
                        Stock = 20,
                        AuthorId = orwell.Id,
                        CategoryId = fantasy.Id,
                        PublisherId = exmo.Id,
                        ImageUrl = "/images/books/animal_farm.jpg"
                    }
                );

                await context.SaveChangesAsync();
                Console.WriteLine("✅ Добавлено 6 книг");
            }
        }
    }
}