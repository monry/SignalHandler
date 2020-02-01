using NUnit.Framework;
using Zenject;

namespace SignalHandler
{
    public class IgnoreSignal : SignalBase<IgnoreSignal>
    {
    }

    public class Subscriptionが無い場合も無視 : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<IgnoreSignal> Publisher { get; }
        [Inject] private ISignalReceiver<IgnoreSignal> Receiver { get; }

        [SetUp]
        public void Install()
        {
            SignalHandler<IgnoreSignal>.InstallSignal(Container, signalMissingHandlerResponses: SignalMissingHandlerResponses.Ignore);

            Container.Inject(this);
        }
    }
}