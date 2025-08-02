using EFAcceleratorTools.Examples.Infrastructure.Data.Context;

namespace EFAcceleratorTools.Test.Fixtures;

public class FixtureBase
{
    public DataContext Context { get; private set; }
    public DataContextFactory ContextFactory { get; private set; }

    protected FixtureBase()
    {
        Context = new DataContextFixture().GetDataContext();
        ContextFactory = new DataContextFixture().GetDataContextFactory();
    }

    public virtual void Reset()
    {
        Context = new DataContextFixture().GetDataContext();
        ContextFactory = new DataContextFixture().GetDataContextFactory();
    }

    public async void CommitAsync()
    {
        await Context.SaveChangesAsync();
    }
}
