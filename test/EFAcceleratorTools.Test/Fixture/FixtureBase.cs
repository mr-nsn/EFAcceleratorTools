using EFAcceleratorTools.Examples.Infrastructure.Data.Context;

namespace EFAcceleratorTools.Test.Fixtures;

public class FixtureBase
{
    public DataContext Context { get; private set; }

    protected FixtureBase()
    {
        Context = new DataContextFixture().ObterDataContext();
    }

    public virtual void Reset()
    {
        Context = new DataContextFixture().ObterDataContext();
    }

    public async void CommitAsync()
    {
        await Context.SaveChangesAsync();
    }
}
