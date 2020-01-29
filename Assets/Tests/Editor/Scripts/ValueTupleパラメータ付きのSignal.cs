using NUnit.Framework;
using UniRx;
using Zenject;

namespace SignalHandler
{
    public class SignalWithValueTupleParameter : SignalBase<SignalWithValueTupleParameter, (bool boolValue, int intValue)>
    {
    }

    public class ExtendedSignalWithValueTupleParameter : SignalWithValueTupleParameter
    {
    }

    public class ValueTupleパラメータ付きのSignal : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<SignalWithValueTupleParameter> Publisher { get; }
        [Inject] private ISignalReceiver<SignalWithValueTupleParameter> Receiver { get; }

        [SetUp]
        public void Install()
        {
            Container.DeclareSignalWithHandler<SignalWithValueTupleParameter>().OptionalSubscriber();
            Container.DeclareSignalWithHandler<SignalWithClassParameter>().OptionalSubscriber();

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(SignalWithValueTupleParameter.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var signalForPublish = SignalWithValueTupleParameter.Create();
            var signalForReceive = SignalWithValueTupleParameter.Create();

            Assert.False(ReferenceEquals(signalForPublish, signalForReceive));
            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void C_継承型は通す()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(ExtendedSignalWithValueTupleParameter.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void D_パラメータの内容が等しい場合は通す()
        {
            var signalForPublish = SignalWithValueTupleParameter.Create((true, 1));
            var signalForReceive = SignalWithValueTupleParameter.Create((true, 1));

            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void E_パラメータの内容が異なる場合は通さない()
        {
            var signalForPublish = SignalWithValueTupleParameter.Create((true, 1));
            var signalForReceive = SignalWithValueTupleParameter.Create((false, 2));

            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Fail("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Pass("Signal を受信しませんでした");
        }

        [Test]
        public void F_送信したパラメータを受け取れる()
        {
            Receiver
                .Receive()
                .Subscribe(
                    signal =>
                    {
                        Assert.AreEqual(true, signal.Parameter.boolValue);
                        Assert.AreEqual(1, signal.Parameter.intValue);
                        Assert.Pass("Signal を受信しました");
                    }
                );

            Publisher.Publish(SignalWithValueTupleParameter.Create((true, 1)));

            Assert.Fail("Signal を受信しませんでした");
        }
    }
}