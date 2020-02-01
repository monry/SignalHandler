using System;
using NSubstitute;
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
            SignalHandlerInstaller<Signal>.Install(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            var mock = Substitute.For<IObserver<Signal>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(Signal.Create());
            Publisher.Publish(Signal.Create());

            mock.Received(2).OnNext(Arg.Any<Signal>());
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var mock = Substitute.For<IObserver<Signal>>();

            var signalToPublish = Signal.Create();
            var signalToReceive = Signal.Create();

            Assert.That(ReferenceEquals(signalToPublish, signalToReceive), Is.False);

            Receiver.Receive(signalToReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalToPublish);

            mock.Received().OnNext(signalToPublish);
        }

        [Test]
        public void C_継承型は通す()
        {
            var mock = Substitute.For<IObserver<Signal>>();

            var extendedSignal = ExtendedSignal.Create();

            Assert.That(extendedSignal, Is.InstanceOf<Signal>());

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(ExtendedSignal.Create());

            mock.Received().OnNext(Arg.Any<Signal>());
        }
    }
}