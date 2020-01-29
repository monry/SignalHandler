using NUnit.Framework;
using SignalHandler.Application.Extension;
using Zenject;

namespace SignalHandler
{
    public class SignalTests : ZenjectUnitTestFixture
    {
        [SetUp]
        private void Install()
        {
            Container.DeclareSignalWithHandler<Signal>().OptionalSubscriber();
            Container.DeclareSignalWithHandler<SignalWithStructParameter>().OptionalSubscriber();
            Container.DeclareSignalWithHandler<SignalWithValueTupleParameter>().OptionalSubscriber();
        }
    }
}