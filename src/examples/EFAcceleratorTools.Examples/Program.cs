using Apparatus.AOT.Reflection;
using EFAcceleratorTools.DataTables;
using EFAcceleratorTools.Examples.Domain.Aggregates.Courses;
using EFAcceleratorTools.Examples.Domain.Aggregates.Instructors;
using EFAcceleratorTools.Examples.Infrastructure.Data;
using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using EFAcceleratorTools.Examples.Infrastructure.Data.Repositories.Aggregates.Courses;
using EFAcceleratorTools.Examples.Infrastructure.IoC;
using EFAcceleratorTools.Models.Builders;
using EFAcceleratorTools.Select.Defaults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Data;
using System.Linq.Expressions;

public class Program
{
    private readonly ICourseRepository _courseRepository;

    public Program(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
        _courseRepository.DisableChangeTracker();
    }

    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        var context = host.Services.GetRequiredService<DataContext>();
        await DbInitializer.SeedAsync(context);

        await host.Services.GetRequiredService<Program>().RunAsync(context);
    }

    public async Task RunAsync(DataContext context)
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
        var queryFilter = new QueryFilterBuilder<Course>(c => c.Title)
            .WithPage(1)
            .WithPageSize(5)
            .WithFilter(c => !string.IsNullOrWhiteSpace(c.Title))
            .WithOrderAscending(c => c.CreatedAt)
            .WithOrderDescending(c => c.InstructorId)
            .WithFields(SelectsDefaults<Course>.BasicFields)
            .Build();
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("SearchWithPaginationAsync:");
        var pagedResult = await _courseRepository.SearchWithPaginationAsync(queryFilter);
        Console.WriteLine(JsonConvert.SerializeObject(pagedResult, jsonSettings));

        // Example: DynamicSelectAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("DynamicSelectAsync:");
        var selectedCourses = await _courseRepository.DynamicSelectAsync(
            filters: new List<Expression<Func<Course, bool>>> { c => !string.IsNullOrWhiteSpace(c.Title) },
            orders: new List<Expression<Func<Course, object?>>> { c => c.Id },
            nameof(Course.Title), nameof(Course.Id)
        );
        Console.WriteLine(JsonConvert.SerializeObject(selectedCourses, jsonSettings));

        // Example: GetAllAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("GetAllAsync:");
        var allCourses = await _courseRepository.GetAllAsync();
        Console.WriteLine(JsonConvert.SerializeObject(allCourses, jsonSettings));

        // Example: GetByIdAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("GetByIdAsync:");
        var courseById = await _courseRepository.GetByIdAsync(originalCourses.ToList()[0].Id);
        Console.WriteLine(JsonConvert.SerializeObject(courseById, jsonSettings));

        // Example: FindAllAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("FindAllAsync:");
        var coursesWithTitle = await _courseRepository.FindAllAsync(c => c.Title != null && c.Title.Contains("C#"));
        Console.WriteLine(JsonConvert.SerializeObject(coursesWithTitle, jsonSettings));

        // Example: FindFirstAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("FindFirstAsync:");
        var firstCourse = await _courseRepository.FindFirstAsync(c => c.Title != null && c.Title.StartsWith("Intro"));
        Console.WriteLine(JsonConvert.SerializeObject(firstCourse, jsonSettings));

        // Example: AddAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        var newCourse = sampleCourses[nextSample];
        await _courseRepository.AddAsync(newCourse);
        Console.WriteLine("AddAsync: Course added (not committed)");

        // Example: AddAndCommitAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("AddAndCommitAsync:");
        var committedCourse = await _courseRepository.AddAndCommitAsync(sampleCourses[nextSample++]);
        Console.WriteLine(JsonConvert.SerializeObject(committedCourse, jsonSettings));

        // Example: AddRangeAsync
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine();
        var coursesToAdd = sampleCourses.Slice(nextSample++, 3);
        await _courseRepository.AddRangeAsync(coursesToAdd);
        Console.WriteLine("AddRangeAsync: Courses added (not committed)");

        // Example: AddRangeAndCommitAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("AddRangeAndCommitAsync:");
        var committedCourses = await _courseRepository.AddRangeAndCommitAsync(sampleCourses.Slice(nextSample++, 3));
        Console.WriteLine(JsonConvert.SerializeObject(committedCourses, jsonSettings));

        // Example: UpdateAsync
        if (courseById != null)
        {
            courseById.Title = "Updated Title";
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            await _courseRepository.UpdateAsync(courseById);
            Console.WriteLine("UpdateAsync: Course updated (not committed)");
        }

        // Example: UpdateAndCommitAsync
        if (courseById != null)
        {
            courseById.Title = "Committed Title";
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            var updatedCourse = await _courseRepository.UpdateAndCommitAsync(courseById);
            Console.WriteLine("UpdateAndCommitAsync:");
            Console.WriteLine(JsonConvert.SerializeObject(updatedCourse, jsonSettings));
        }

        // Example: UpdateRangeAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        await _courseRepository.UpdateRangeAsync(coursesToAdd);
        Console.WriteLine("UpdateRangeAsync: Courses updated (not committed)");

        // Example: UpdateRangeAndCommitAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("UpdateRangeAndCommitAsync:");
        var updatedCourses = await _courseRepository.UpdateRangeAndCommitAsync(coursesToAdd);
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
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        bool anyCourse = await _courseRepository.AnyAsync(c => c.Title == "Course 1");
        Console.WriteLine($"AnyAsync: Is there a course with title 'Course 1'? {anyCourse}");

        // Example: Detach
        if (courseById != null)
        {
            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
            _courseRepository.Detach(courseById);
            Console.WriteLine("Detach: Course detached from context");
        }

        // Example: DetachAll
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        _courseRepository.DetachAll();
        Console.WriteLine("DetachAll: All courses detached from context");

        // Example: EnableChangeTracker
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        _courseRepository.EnableChangeTracker();
        Console.WriteLine("EnableChangeTracker: Change Tracker enabled");

        // Example: DisableChangeTracker
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        _courseRepository.DisableChangeTracker();
        Console.WriteLine("DisableChangeTracker: Change Tracker disabled");

        // Example: CommitAsync
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        int changes = await _courseRepository.CommitAsync();
        Console.WriteLine($"CommitAsync: {changes} changes saved");

        // Example: MassiveQueryAsync (specific method of ICourseRepository)
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("MassiveQueryAsync:");
        var massiveResult = await _courseRepository.MassiveQueryAsync();
        Console.WriteLine(JsonConvert.SerializeObject(massiveResult, jsonSettings));

        // Example: ToDataTable
        Console.WriteLine();
        Console.WriteLine("------------------------------------------------------------------------------------------------------------");
        Console.WriteLine();
        Console.WriteLine("ToDataTable:");
        var columnsOrder = new Dictionary<string, int>()
        {
            { "Id", 0 },
            { "FullName", 1 },
            { "CreatedAt", 2 }
        };

        var instructorProc = new Instructor
        {
            FullName = "Instructor PROC"
        };

        var instructorDataTable = new List<Instructor>
        {
            instructorProc
        }.ToDataTable(columnsOrder, context);
        Console.WriteLine(JsonConvert.SerializeObject(instructorDataTable.Rows, jsonSettings));

        var parametersAdd = new SqlParameter[]
        {
            new SqlParameter("@INSTRUCTORS", SqlDbType.Structured)
            {
                TypeName = "dbo.TP_TB_INSTRUCTOR",
                Value = instructorDataTable
            }
        };

        //await context.Database.ExecuteSqlRawAsync($"EXEC [dbo].[USP_ADD_INSTRUCTOR] " + $"@INSTRUCTORS", parametersAdd);

        //var parametersRemove = new SqlParameter[]
        //{
        //    new SqlParameter("@FULLNAME", instructorProc.FullName)
        //};

        //await context.Database.ExecuteSqlRawAsync($"EXEC [dbo].[USP_REMOVE_INSTRUCTOR] " + $"@FULLNAME", parametersRemove);

        // Cleanup: Remove all courses added during the examples ---
        var allAfterExamples = await _courseRepository.GetAllAsync();
        var addedCourses = allAfterExamples.Where(c => !originalIds.Contains(c.Id)).ToList();

        await _courseRepository.RemoveRangeCascadeAsync(addedCourses.Select(x => x.Id).ToArray());        

        changes = await _courseRepository.CommitAsync();
        Console.WriteLine($"Cleanup: {changes} changes saved");
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