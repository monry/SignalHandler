using NUnit.Framework;
using Zenject;

namespace SignalHandler
{
    public class ThrowSignal : SignalBase<ThrowSignal>
    {
    }

    public class Subscriptionが無い場合に例外 : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<ThrowSignal> Publisher { get; }
        [Inject] private ISignalReceiver<ThrowSignal> Receiver { get; }

        [SetUp]
        public void Install()
        {
            SignalHandler<ThrowSignal>.InstallSignal(Container, signalMissingHandlerResponses: SignalMissingHandlerResponses.Throw);

            Container.Inject(this);
        }
    }
}