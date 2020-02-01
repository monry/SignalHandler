using System;
using NSubstitute;
using NUnit.Framework;
using UniRx;
using Zenject;

namespace SignalHandler
{
    public class SingleSignal : SignalBase<SingleSignal, int>, ISignalTerminator
    {
        bool ISignalTerminator.IsTerminator => true;
    }

    public class Publish後に即CompletedするSignal : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<SingleSignal> Publisher { get; }
        [Inject] private ISignalReceiver<SingleSignal> Receiver { get; }

        [SetUp]
        public void Install()
        {
            SignalHandler<SingleSignal>.InstallSignal(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_OnCompletedが即送信される()
        {
            var mock = Substitute.For<IObserver<SingleSignal>>();

            Receiver.Receive().DoOnCompleted(mock.OnCompleted).Subscribe();

            Publisher.Publish(SingleSignal.Create());

            mock.Received(1).OnCompleted();
        }

        [Test]
        public void B_複数回送信しても初回のSignalのみが送信される()
        {
            var mock = Substitute.For<IObserver<SingleSignal>>();

            Receiver.Receive().DoOnCompleted(mock.OnCompleted).Subscribe(signal => Assert.That(signal.Parameter, Is.EqualTo(1)));

            Publisher.Publish(SingleSignal.Create(1));
            // ココで Subscription がないという警告が出る
            Publisher.Publish(SingleSignal.Create(2));

            mock.Received(1).OnCompleted();
        }

        [Test]
        public void C_完了後には受信できない()
        {
            var mock = Substitute.For<IObserver<SingleSignal>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(SingleSignal.Create());

            Receiver.Receive().Subscribe(mock.OnNext);

            mock.Received(1).OnNext(Arg.Any<SingleSignal>());
        }
    }
}