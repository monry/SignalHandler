using NUnit.Framework;
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
            SignalHandler<Signal>.InstallSignal(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            var count = 0;

            Receiver.Receive().Subscribe(_ => count++);

            Publisher.Publish(Signal.Create());
            Publisher.Publish(Signal.Create());

            Assert.That(count, Is.EqualTo(2), "Signal を正しく受信しませんでした");
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var result = false;

            var signalToPublish = Signal.Create();
            var signalToReceive = Signal.Create();

            Receiver.Receive(signalToReceive).Subscribe(_ => result = true);

            Publisher.Publish(signalToPublish);

            Assert.That(ReferenceEquals(signalToPublish, signalToReceive), Is.False, "インスタンスが異なっていません");
            Assert.That(result, Is.True, "Signal を受信しませんでした");
        }

        [Test]
        public void C_継承型は通す()
        {
            var result = false;

            var extendedSignal = ExtendedSignal.Create();

            Receiver.Receive().Subscribe(_ => result = true);

            Publisher.Publish(ExtendedSignal.Create());

            Assert.That(extendedSignal, Is.InstanceOf<Signal>(), "ExtendedSignal が継承型ではありません");
            Assert.That(result, Is.True, "Signal を受信しませんでした");
        }
    }
}