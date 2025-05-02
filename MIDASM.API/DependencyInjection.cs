using Microsoft.EntityFrameworkCore;
using MIDASM.Application;
using MIDASM.Application.Services.Crypto;
using MIDASM.Contract;
using MIDASM.Domain.Entities;
using MIDASM.Infrastructure;
using MIDASM.Persistence;

namespace MIDASM.API;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureDependencyLayers(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        return services
            .ConfigureInfrastructureServices()
            .ConfigureContractLayer()
            .ConfigureApplicationLayer()
            .ConfigurePersistenceLayer();
    }
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var rsaEncryptService = scope.ServiceProvider.GetRequiredKeyedService<ICryptoService>("RSA");
                                context.Database.MigrateAsync().GetAwaiter().GetResult();
        await SeedAsync(context, rsaEncryptService);
    }

    public static async Task SeedAsync(ApplicationDbContext context, ICryptoService rsaEncryptService)
    {
        if ((await context.Categories.IgnoreQueryFilters().CountAsync()) == 0)
        {
            context.Categories.AddRange(
                new Category { Id = Guid.Parse("a3f1c2d4-5b6e-7f80-1234-56789abcdef0"), Name = "Technology", Description = "Latest news, tutorials, and updates about technology." },
                new Category { Id = Guid.Parse("b4e2d3c5-6f7a-8b90-2345-6789abcdef01"), Name = "Science", Description = "Discoveries, research articles, and scientific insights." },
                new Category { Id = Guid.Parse("c5d3e4b6-7a8b-9c01-3456-789abcdef012"), Name = "Sports", Description = "Latest scores, analyses, and commentary on sports events." },
                new Category { Id = Guid.Parse("d6e4f5c7-8b9c-0d12-4567-89abcdef0123"), Name = "Entertainment", Description = "Movie reviews, music releases, and celebrity news." },
                new Category { Id = Guid.Parse("e7f5a6d8-9c0d-1e23-5678-9abcdef01234"), Name = "Education", Description = "Study tips, course recommendations, and learning resources." },
                new Category { Id = Guid.Parse("f8a6b7e9-0d1e-2f34-6789-abcdef012345"), Name = "Health", Description = "Wellness advice, medical news, and healthy living guides." },
                new Category { Id = Guid.Parse("0a1b2c3d-4e5f-6a78-789a-bcdef0123456"), Name = "Travel", Description = "Destination guides, travel tips, and itinerary ideas." },
                new Category { Id = Guid.Parse("1b2c3d4e-5f6a-7b89-89ab-cdef01234567"), Name = "Food", Description = "Recipes, restaurant reviews, and culinary trends." },
                new Category { Id = Guid.Parse("2c3d4e5f-6a7b-8c90-9abc-def012345678"), Name = "Business", Description = "Market analysis, startup tips, and corporate news." },
                new Category { Id = Guid.Parse("3d4e5f6a-7b8c-9d01-abcd-ef0123456789"), Name = "Beauty"});
            await context.SaveChangesAsync();
        }

        if ((await context.Books.IgnoreQueryFilters().CountAsync()) == 0)
        {
            context.Books.AddRange(new Book { Id = Guid.Parse("9a7f1e2d-3c4b-5a6f-7e8d-9c0b1a2d3e4f"), Title = "Introduction to Programming", Description = "A beginner's guide to programming concepts and techniques.", Author = "John Doe", Quantity = 10, Available = 10, CategoryId = Guid.Parse("a3f1c2d4-5b6e-7f80-1234-56789abcdef0") },
                        new Book { Id = Guid.Parse("8b6e2d3c-4f5a-6b7c-8d9e-0f1a2b3c4d5e"), Title = "Advanced C# Programming", Description = "In‑depth coverage of C# language features and best practices.", Author = "Jane Smith", Quantity = 15, Available = 12, CategoryId = Guid.Parse("a3f1c2d4-5b6e-7f80-1234-56789abcdef0") },
                        new Book { Id = Guid.Parse("7c5d3e4b-6a7b-8c9d-0e1f-2a3b4c5d6e7f"), Title = "Deep Learning with Python", Description = "Practical guide to building neural networks using Python libraries.", Author = "Alice Johnson", Quantity = 8, Available = 8, CategoryId = Guid.Parse("b4e2d3c5-6f7a-8b90-2345-6789abcdef01") },
                        new Book { Id = Guid.Parse("6d4e5f6a-7b8c-9d0e-1f2a-3b4c5d6e7f8a"), Title = "Fundamentals of Physics", Description = "Core principles of mechanics, thermodynamics, and electromagnetism.", Author = "Robert Brown", Quantity = 12, Available = 11, CategoryId = Guid.Parse("b4e2d3c5-6f7a-8b90-2345-6789abcdef01") },
                        new Book { Id = Guid.Parse("5e3f6a7b-8c9d-0e1f-2a3b-4c5d6e7f8a9b"), Title = "Champions League: The History", Description = "An exhaustive history of UEFA Champions League matches and legends.", Author = "Michael Green", Quantity = 20, Available = 18, CategoryId = Guid.Parse("c5d3e4b6-7a8b-9c01-3456-789abcdef012") },
                        new Book { Id = Guid.Parse("4f2a7b8c-9d0e-1f2a-3b4c-5d6e7f8a9b0c"), Title = "Sports Analytics 101", Description = "Introduction to data analysis techniques in sports.", Author = "Laura White", Quantity = 7, Available = 7, CategoryId = Guid.Parse("c5d3e4b6-7a8b-9c01-3456-789abcdef012") },
                        new Book { Id = Guid.Parse("3e1f8a9b-0c1d-2e3f-4a5b-6c7d8e9f0a1b"), Title = "Hollywood Story", Description = "Behind‑the‑scenes look at Hollywood's golden era.", Author = "Emma Davis", Quantity = 9, Available = 5, CategoryId = Guid.Parse("d6e4f5c7-8b9c-0d12-4567-89abcdef0123") },
                        new Book { Id = Guid.Parse("2d0e9b1a-2b3c-4d5e-6f7a-8b9c0d1e2f3a"), Title = "Streaming Platforms Explained", Description = "Comparison and evolution of modern streaming services.", Author = "William King", Quantity = 6, Available = 4, CategoryId = Guid.Parse("d6e4f5c7-8b9c-0d12-4567-89abcdef0123") },
                        new Book { Id = Guid.Parse("1c9e0d2b-3c4d-5e6f-7a8b-9c0d1e2f3a4b"), Title = "Effective Teaching Strategies", Description = "Techniques and methods for impactful classroom instruction.", Author = "Sophia Lee", Quantity = 14, Available = 14, CategoryId = Guid.Parse("e7f5a6d8-9c0d-1e23-5678-9abcdef01234") },
                        new Book { Id = Guid.Parse("0b8d1c3f-4d5e-6f7a-8b9c-0d1e2f3a4b5c"), Title = "Educational Psychology", Description = "Study of how people learn and retain information.", Author = "Daniel Martinez", Quantity = 11, Available = 9, CategoryId = Guid.Parse("e7f5a6d8-9c0d-1e23-5678-9abcdef01234") },
                        new Book { Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), Title = "Nutrition and Wellness", Description = "Guidelines for healthy eating and lifestyle.", Author = "Olivia Thompson", Quantity = 13, Available = 13, CategoryId = Guid.Parse("f8a6b7e9-0d1e-2f34-6789-abcdef012345") },
                        new Book { Id = Guid.Parse("223e4567-e89b-12d3-a456-426614174001"), Title = "The Healthy Mind", Description = "Mental health practices for everyday life.", Author = "Liam Anderson", Quantity = 10, Available = 8, CategoryId = Guid.Parse("f8a6b7e9-0d1e-2f34-6789-abcdef012345") },
                        new Book { Id = Guid.Parse("323e4567-e89b-12d3-a456-426614174002"), Title = "Backpacking Europe", Description = "Planning routes and tips for budget travel across Europe.", Author = "Emily Wilson", Quantity = 16, Available = 16, CategoryId = Guid.Parse("0a1b2c3d-4e5f-6a78-789a-bcdef0123456") },
                        new Book { Id = Guid.Parse("423e4567-e89b-12d3-a456-426614174003"), Title = "Adventure Travel Guide", Description = "Adventure sports and off‑the‑beaten‑path destinations.", Author = "Noah Taylor", Quantity = 5, Available = 2, CategoryId = Guid.Parse("0a1b2c3d-4e5f-6a78-789a-bcdef0123456") },
                        new Book { Id = Guid.Parse("523e4567-e89b-12d3-a456-426614174004"), Title = "Culinary Arts: A Beginner's Guide", Description = "Fundamentals of cooking techniques and kitchen tools.", Author = "Ava Moore", Quantity = 18, Available = 17, CategoryId = Guid.Parse("1b2c3d4e-5f6a-7b89-89ab-cdef01234567") },
                        new Book { Id = Guid.Parse("623e4567-e89b-12d3-a456-426614174005"), Title = "World Cuisines", Description = "Exploration of global culinary traditions and recipes.", Author = "Mason Jackson", Quantity = 7, Available = 3, CategoryId = Guid.Parse("1b2c3d4e-5f6a-7b89-89ab-cdef01234567") },
                        new Book { Id = Guid.Parse("723e4567-e89b-12d3-a456-426614174006"), Title = "Startup Funding Essentials", Description = "Strategies for securing investment in startups.", Author = "Isabella Martin", Quantity = 12, Available = 11, CategoryId = Guid.Parse("2c3d4e5f-6a7b-8c90-9abc-def012345678") },
                        new Book { Id = Guid.Parse("823e4567-e89b-12d3-a456-426614174007"), Title = "Market Research Methods", Description = "Qualitative and quantitative research techniques.", Author = "Ethan Lee", Quantity = 9, Available = 9, CategoryId = Guid.Parse("2c3d4e5f-6a7b-8c90-9abc-def012345678") },
                        new Book { Id = Guid.Parse("923e4567-e89b-12d3-a456-426614174008"), Title = "Skincare Routines", Description = "Daily and weekly skincare practices for healthy skin.", Author = "Charlotte Brown", Quantity = 8, Available = 6, CategoryId = Guid.Parse("3d4e5f6a-7b8c-9d01-abcd-ef0123456789") },
                        new Book { Id = Guid.Parse("a23e4567-e89b-12d3-a456-426614174009"), Title = "Makeup Masterclass", Description = "Step‑by‑step makeup tutorials and product recommendations.", Author = "Lucas Wilson", Quantity = 10, Available = 10, CategoryId = Guid.Parse("3d4e5f6a-7b8c-9d01-abcd-ef0123456789") });
            await context.SaveChangesAsync();
        }

        if ((await context.Users.IgnoreQueryFilters().CountAsync()) == 0)
        {
            var adminRole = new Role { Name = "Admin" };
            var userRole = new Role { Name = "User" };

            context.Roles.AddRange(adminRole, userRole);
            context.Users.Add(new User
            {
                Username = "admin",
                Password = rsaEncryptService.Encrypt("admin123"),
                FirstName = "admin",
                LastName = "admin",
                Email = "admin@parabook.com",
                BookBorrowingLimit = 3,
                RoleId = adminRole.Id
            });
            await context.SaveChangesAsync();
        }
    }
}
