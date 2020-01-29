using NUnit.Framework;
using UniRx;
using Zenject;

namespace SignalHandler
{
    public class SignalWithStructParameter : SignalBase<SignalWithStructParameter, SignalWithStructParameter.StructParameter>
    {
        public struct StructParameter
        {
            public bool BoolValue { get; }
            public int IntValue { get; }

            public StructParameter(bool boolValue, int intValue)
            {
                BoolValue = boolValue;
                IntValue = intValue;
            }
        }
    }

    public class ExtendedSignalWithStructParameter : SignalWithStructParameter
    {
    }

    public class 構造体パラメータ付きのSignal : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<SignalWithStructParameter> Publisher { get; }
        [Inject] private ISignalReceiver<SignalWithStructParameter> Receiver { get; }

        [SetUp]
        public void Install()
        {
            Container.DeclareSignalWithHandler<SignalWithStructParameter>().OptionalSubscriber();
            Container.DeclareSignalWithHandler<SignalWithClassParameter>().OptionalSubscriber();

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(SignalWithStructParameter.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var signalForPublish = SignalWithStructParameter.Create();
            var signalForReceive = SignalWithStructParameter.Create();

            Assert.False(ReferenceEquals(signalForPublish, signalForReceive));
            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void C_継承型は通す()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(ExtendedSignalWithStructParameter.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void D_パラメータの内容が等しい場合は通す()
        {
            var signalForPublish = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1));
            var signalForReceive = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1));

            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void E_パラメータの内容が異なる場合は通さない()
        {
            var signalForPublish = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1));
            var signalForReceive = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(false, 2));

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
                        Assert.AreEqual(true, signal.Parameter.BoolValue);
                        Assert.AreEqual(1, signal.Parameter.IntValue);
                        Assert.Pass("Signal を受信しました");
                    }
                );

            Publisher.Publish(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1)));

            Assert.Fail("Signal を受信しませんでした");
        }
    }
}