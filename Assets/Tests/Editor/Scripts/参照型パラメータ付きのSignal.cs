using NUnit.Framework;
using UniRx;
using Zenject;

namespace SignalHandler
{
    public class SignalWithClassParameter : SignalBase<SignalWithClassParameter, SignalWithClassParameter.ClassParameter>
    {
        public class ClassParameter
        {
            public bool BoolValue { get; }
            public int IntValue { get; }

            public ClassParameter(bool boolValue, int intValue)
            {
                BoolValue = boolValue;
                IntValue = intValue;
            }
        }
    }

    public class ExtendedSignalWithClassParameter : SignalWithClassParameter
    {
    }

    public class 参照型パラメータ付きのSignal : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<SignalWithClassParameter> Publisher { get; }
        [Inject] private ISignalReceiver<SignalWithClassParameter> Receiver { get; }

        [SetUp]
        public void Install()
        {
            SignalHandler<SignalWithClassParameter>.InstallSignal(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(SignalWithClassParameter.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var signalForPublish = SignalWithClassParameter.Create();
            var signalForReceive = SignalWithClassParameter.Create();

            Assert.False(ReferenceEquals(signalForPublish, signalForReceive));
            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void C_継承型は通す()
        {
            Receiver.Receive().Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(ExtendedSignalWithClassParameter.Create());

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void D_パラメータの参照が等しい場合は通す()
        {
            var classParameter = new SignalWithClassParameter.ClassParameter(true, 1);
            var signalForPublish = SignalWithClassParameter.Create(classParameter);
            var signalForReceive = SignalWithClassParameter.Create(classParameter);

            Receiver.Receive(signalForReceive).Subscribe(_ => Assert.Pass("Signal を受信しました"));

            Publisher.Publish(signalForPublish);

            Assert.Fail("Signal を受信しませんでした");
        }

        [Test]
        public void E_パラメータの参照が異なる場合は通さない()
        {
            var classParameterForPublish = new SignalWithClassParameter.ClassParameter(true, 1);
            var classParameterForReceive = new SignalWithClassParameter.ClassParameter(true, 1);
            var signalForPublish = SignalWithClassParameter.Create(classParameterForPublish);
            var signalForReceive = SignalWithClassParameter.Create(classParameterForReceive);

            Assert.False(ReferenceEquals(classParameterForPublish, classParameterForReceive));
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

            Publisher.Publish(SignalWithClassParameter.Create(new SignalWithClassParameter.ClassParameter(true, 1)));

            Assert.Fail("Signal を受信しませんでした");
        }
    }
}