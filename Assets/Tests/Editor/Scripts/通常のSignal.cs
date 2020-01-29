using NUnit.Framework;
using SignalHandler.Application.Extension;
using UniRx;
using Zenject;

namespace SignalHandler
{
    public class Signal : SignalBase<Signal>
    {
    }

    public class ExtendedSignal : Signal
    {
    }

    public class 通常のSignal : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<Signal> Publisher { get; }
        [Inject] private ISignalReceiver<Signal> Receiver { get; }

        [SetUp]
        public void Install()
        {
            Container.DeclareSignalWithHandler<Signal>().OptionalSubscriber();

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(Signal.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var signalToPublish = Signal.Create();
            var signalToReceive = Signal.Create();

            Assert.False(ReferenceEquals(signalToPublish, signalToReceive));
            Receiver.Receive(signalToReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalToPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void C_継承型は通す()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(ExtendedSignal.Create());

            Assert.Fail("Signal を受信しませんでした");
        }
    }
}