using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Infrastructure.Data;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;
using EFAcceleratorTools.Examples.Infrastructure.IoC;
using EFAcceleratorTools.Models.Builders;
using EFAcceleratorTools.Select.Defaults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

public class Program
{
    private readonly ICourseRepository _courseRepository;

    public Program(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        
        var context = host.Services.GetRequiredService<DataContext>();
        await DbInitializer.SeedAsync(context);

        await host.Services.GetRequiredService<Program>().RunAsync();
    }

    public async Task RunAsync()
    {
        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        // Capture IDs of existing courses before running the examples
        var originalCourses = await _courseRepository.GetAllAsync();
        var originalIds = originalCourses.Select(c => c.Id).ToHashSet();

        var sampleCourses = DbInitializer.GenerateCourses(50, new Random().Next());
        var nextSample = 0;

        // Example: SearchWithPaginationAsync
        var queryFilter = new QueryFilterBuilder<Course>()
            .WithPage(1)
            .WithPageSize(2)
            .WithFields(SelectsDefaults<Course>.BasicFields)
            .Build();
        var pagedResult = await _courseRepository.SearchWithPaginationAsync(queryFilter);
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("SearchWithPaginationAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(pagedResult, jsonSettings));

        // Example: DynamicSelectAsync
        var selectedCourses = await _courseRepository.DynamicSelectAsync(nameof(Course.Title), nameof(Course.Id));
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("DynamicSelectAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(selectedCourses, jsonSettings));
        
        // Example: GetAllAsync
        var allCourses = await _courseRepository.GetAllAsync();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("GetAllAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(allCourses, jsonSettings));
        
        // Example: GetByIdAsync
        var courseById = await _courseRepository.GetByIdAsync(originalCourses.ToList()[0].Id);
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("GetByIdAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(courseById, jsonSettings));
        
        // Example: FindAllAsync
        var coursesWithTitle = await _courseRepository.FindAllAsync(c => c.Title != null && c.Title.Contains("C#"));
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("FindAllAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(coursesWithTitle, jsonSettings));
        
        // Example: FindFirstAsync
        var firstCourse = await _courseRepository.FindFirstAsync(c => c.Title != null && c.Title.StartsWith("Intro"));
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("FindFirstAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(firstCourse, jsonSettings));
        
        // Example: AddAsync
        var newCourse = sampleCourses[nextSample];
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        await _courseRepository.AddAsync(newCourse);
        Console.WriteLine("AddAsync: Course added (not committed)");
        
        // Example: AddAndCommitAsync
        var committedCourse = await _courseRepository.AddAndCommitAsync(sampleCourses[nextSample++]);
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("AddAndCommitAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(committedCourse, jsonSettings));

        // Example: AddRangeAsync
        var coursesToAdd = sampleCourses.Slice(nextSample++, 3);
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine();
        await _courseRepository.AddRangeAsync(coursesToAdd);
        Console.WriteLine("AddRangeAsync: Courses added (not committed)");
        
        // Example: AddRangeAndCommitAsync
        var committedCourses = await _courseRepository.AddRangeAndCommitAsync(sampleCourses.Slice(nextSample++, 3));
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("AddRangeAndCommitAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(committedCourses, jsonSettings));
        
        // Example: UpdateAsync
        if (courseById != null)
        {
            courseById.Title = "Updated Title";
            await _courseRepository.UpdateAsync(courseById);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("UpdateAsync: Course updated (not committed)");
        }

        // Example: UpdateAndCommitAsync
        if (courseById != null)
        {
            courseById.Title = "Committed Title";
            var updatedCourse = await _courseRepository.UpdateAndCommitAsync(courseById);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("UpdateAndCommitAsync:");
            Console.WriteLine(JsonConvert.SerializeObject(updatedCourse, jsonSettings));
        }

        // Example: UpdateRangeAsync
        await _courseRepository.UpdateRangeAsync(coursesToAdd);
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("UpdateRangeAsync: Courses updated (not committed)");
        
        // Example: UpdateRangeAndCommitAsync
        var updatedCourses = await _courseRepository.UpdateRangeAndCommitAsync(coursesToAdd);
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("UpdateRangeAndCommitAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(updatedCourses, jsonSettings));

        // Only removes the main entity, not related entities.
        // Example: RemoveAsync
        //var courseId = sampleCourses[0].Id;
        //await _courseRepository.RemoveAsync(courseId);
        //Console.WriteLine("RemoveAsync: Course removed (not committed)");

        // Example: RemoveAndCommitAsync
        //await _courseRepository.RemoveAndCommitAsync(sampleCourses[1].Id);
        //Console.WriteLine("RemoveAndCommitAsync: Course removed and committed");

        // Example: AnyAsync
        bool anyCourse = await _courseRepository.AnyAsync(c => c.Title == "Course 1");
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"AnyAsync: Is there a course with title 'Course 1'? {anyCourse}");
        
        // Example: Detach
        if (courseById != null)
        {
            _courseRepository.Detach(courseById);
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("Detach: Course detached from context");
        }

        // Example: DetachAll
        _courseRepository.DetachAll();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("DetachAll: All courses detached from context");
        
        // Example: DisableChangeTracker
        _courseRepository.DisableChangeTracker();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("DisableChangeTracker: Change Tracker disabled");
        
        // Example: EnableChangeTracker
        _courseRepository.EnableChangeTracker();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("EnableChangeTracker: Change Tracker enabled");
        
        // Example: CommitAsync
        int changes = await _courseRepository.CommitAsync();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine($"CommitAsync: {changes} changes saved");
        
        // Example: MassiveQueryAsync (specific method of ICourseRepository)
        var massiveResult = await _courseRepository.MassiveQueryAsync();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("MassiveQueryAsync:");
        Console.WriteLine(JsonConvert.SerializeObject(massiveResult, jsonSettings));

        // --- Cleanup: Remove all courses added during the examples ---
        var allAfterExamples = await _courseRepository.GetAllAsync();
        var addedCourses = allAfterExamples.Where(c => !originalIds.Contains(c.Id)).ToList();
                
        await _courseRepository.RemoveRangeCascadeAndCommitAsync(addedCourses.Select(x => x.Id).ToArray());
        Console.WriteLine();
        Console.WriteLine("Cleanup: All courses added during the examples have been removed.");
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = new ConfigurationBuilder();
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var config = builder.Build();

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.RegisterServices(config);
            });
    }  
}