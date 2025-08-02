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
        var queryFilter = new QueryFilterBuilder<Course>()
            .WithPage(1)
            .WithPageSize(2)
            .WithFields(SelectsDefaults<Course>.BasicFields)
            .Build();

        var result = await _courseRepository.SearchWithPaginationAsync(queryFilter);

        var jsonSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        Console.WriteLine(JsonConvert.SerializeObject(result, jsonSettings));
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