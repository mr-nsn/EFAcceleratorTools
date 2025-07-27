using AutoFixture;
using AutoFixture.Xunit2;

namespace EFAcceleratorTools.Test.Customization;

public class AutoDataCustomAttribute : AutoDataAttribute
{
    public AutoDataCustomAttribute() : base(() => new Fixture().Customize(new NoCircularReferencesCustomization()))
    {

    }
}
