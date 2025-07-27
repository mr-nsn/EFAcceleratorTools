using AutoFixture;

namespace EFAcceleratorTools.Test.Customization;

public class NoCircularReferencesCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {   
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(behavior => fixture.Behaviors.Remove(behavior));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}
