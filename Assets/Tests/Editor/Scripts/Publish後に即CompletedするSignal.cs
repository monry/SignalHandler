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
            Container.DeclareSignalWithHandler<SingleSignal>().OptionalSubscriber();

            Container.Inject(this);
        }

        [Test]
        public void A_OnCompletedが即送信される()
        {
            var called = false;

            Receiver.Receive().DoOnCompleted(() => called = true).Subscribe();

            Publisher.Publish(SingleSignal.Create());

            Assert.That(called, Is.True, "Signal を受信しませんでした");
        }

        [Test]
        public void B_複数回送信しても初回のSignalのみが送信される()
        {
            var count = 0;
            var receivedSignal = default(SingleSignal);

            Receiver.Receive().DoOnCompleted(() => count++).Subscribe(signal => receivedSignal = signal);

            Publisher.Publish(SingleSignal.Create(1));
            Publisher.Publish(SingleSignal.Create(2));

            Assert.That(count, Is.EqualTo(1), "Signal を受信しませんでした");
            Assert.That(receivedSignal.Parameter, Is.EqualTo(1), "2つ目の Signal を受信しています");
        }

        [Test]
        public void C_完了後には受信できない()
        {
            var count = 0;

            Receiver.Receive().Subscribe(_ => count++);

            Publisher.Publish(SingleSignal.Create());

            Receiver.Receive().Subscribe(_ => count++);

            Assert.That(count, Is.EqualTo(1), "Signal を完了後に受信しています");
        }
    }
}