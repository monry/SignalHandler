using NUnit.Framework;
using Zenject;

namespace SignalHandler
{
    public class WarnSignal : SignalBase<WarnSignal>
    {
    }

    public class Subscriptionが無い場合に警告 : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<WarnSignal> Publisher { get; }
        [Inject] private ISignalReceiver<WarnSignal> Receiver { get; }

        [SetUp]
        public void Install()
        {
            // ReSharper disable once RedundantArgumentDefaultValue 書き味を揃えるためにデフォルト引数を明示
            SignalHandler<WarnSignal>.InstallSignal(Container, signalMissingHandlerResponses: SignalMissingHandlerResponses.Warn);

            Container.Inject(this);
        }
    }
}