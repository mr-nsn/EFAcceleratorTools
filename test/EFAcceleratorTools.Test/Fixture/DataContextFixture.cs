using EFAcceleratorTools.Examples.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace EFAcceleratorTools.Test.Fixtures;

public class DataContextFixture
{
    public DataContext ObterDataContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        return context;
    }
}
