using Bogus;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Examples.Util.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Examples.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(DataContext context)
    {
        if (context.Database.IsRelational())
            context.Database.Migrate();

        if (context.Courses.Any())
            return;        

        await context.Courses.AddRangeAsync(GenerateCourses());
        context.SaveChanges();
    }

    public static List<Course> GenerateCourses(int count = 10)
    {
        Randomizer.Seed = new Random(8675309);

        var profileFaker = new Faker<Profile>("pt_BR")
            .RuleFor(p => p.Id, f => 0)
            .RuleFor(p => p.Bio, f => f.Lorem.Paragraph())
            .RuleFor(p => p.LinkedInUrl, f => $"https://www.linkedin.com/in/{f.Internet.UserName()}")
            .RuleFor(p => p.CreatedAt, f => DateTime.UtcNow);

        var instructorFaker = new Faker<Instructor>("pt_BR")
            .RuleFor(i => i.Id, f => 0)
            .RuleFor(i => i.FullName, f => f.Name.FullName())
            .RuleFor(i => i.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(i => i.Profile, f => profileFaker.Generate());

        var lessonFaker = new Faker<Lesson>("pt_BR")
            .RuleFor(l => l.Id, f => 0)
            .RuleFor(l => l.Title, f => f.Commerce.ProductName())
            .RuleFor(l => l.Duration, f => TimeSpan.FromMinutes(f.Random.Int(5, 90)))
            .RuleFor(l => l.CreatedAt, f => DateTime.UtcNow);

        var moduleFaker = new Faker<Module>("pt_BR")
            .RuleFor(m => m.Id, f => 0)
            .RuleFor(m => m.Name, f => f.Company.CatchPhrase())
            .RuleFor(m => m.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(m => m.Lessons, (f, m) => lessonFaker.Generate(f.Random.Int(2, 6)));

        var courseFaker = new Faker<Course>("pt_BR")
            .RuleFor(c => c.Id, f => 0)
            .RuleFor(c => c.Title, f => f.Company.Bs().ToFirstUpper())
            .RuleFor(c => c.CreatedAt, f => DateTime.UtcNow)
            .RuleFor(c => c.Instructor, f => instructorFaker.Generate())
            .RuleFor(c => c.Modules, f => moduleFaker.Generate(f.Random.Int(1, 4)));

        return courseFaker.Generate(count);
    }
}
