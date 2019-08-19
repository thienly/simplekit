using AutoFixture;
using AutoFixture.AutoMoq;

namespace Test.SimpleKit.Base
{
    public abstract class SimpleKitTestBase
    {
        public IFixture Fixture;

        public SimpleKitTestBase()
        {
            Fixture = new Fixture();
            Fixture.Customize(new AutoMoqCustomization());
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}