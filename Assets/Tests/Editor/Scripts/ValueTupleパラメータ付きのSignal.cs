using System;
using NSubstitute;
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
            SignalHandler<SignalWithValueTupleParameter>.InstallSignal(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(SignalWithValueTupleParameter.Create());
            Publisher.Publish(SignalWithValueTupleParameter.Create());

            mock.Received(2).OnNext(Arg.Any<SignalWithValueTupleParameter>());
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            var signalForPublish = SignalWithValueTupleParameter.Create();
            var signalForReceive = SignalWithValueTupleParameter.Create();

            Assert.False(ReferenceEquals(signalForPublish, signalForReceive));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.Received().OnNext(signalForPublish);
        }

        [Test]
        public void C_継承型は通す()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(ExtendedSignalWithValueTupleParameter.Create());

            mock.Received().OnNext(Arg.Any<SignalWithValueTupleParameter>());
        }

        [Test]
        public void D_パラメータの内容が等しい場合は通す()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            var signalForPublish = SignalWithValueTupleParameter.Create((true, 1));
            var signalForReceive = SignalWithValueTupleParameter.Create((true, 1));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.Received(1).OnNext(signalForPublish);
        }

        [Test]
        public void E_パラメータの内容が異なる場合は通さない()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            var signalForPublish = SignalWithValueTupleParameter.Create((true, 1));
            var signalForReceive = SignalWithValueTupleParameter.Create((false, 2));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.DidNotReceive().OnNext(signalForPublish);
        }

        [Test]
        public void F_送信したパラメータを受け取れる()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);
            Receiver
                .Receive()
                .Subscribe(
                    signal =>
                    {
                        Assert.That(signal.Parameter.boolValue, Is.True);
                        Assert.That(signal.Parameter.intValue, Is.EqualTo(1));
                    }
                );

            Publisher.Publish(SignalWithValueTupleParameter.Create((true, 1)));
            Publisher.Publish(SignalWithValueTupleParameter.Create((true, 1)));

            mock.Received(2).OnNext(SignalWithValueTupleParameter.Create((true, 1)));
        }

        [Test]
        public void G_パラメータでフィルタできる()
        {
            var mock = Substitute.For<IObserver<SignalWithValueTupleParameter>>();

            Receiver.Receive((true, 1)).Subscribe(mock.OnNext);

            Publisher.Publish(SignalWithValueTupleParameter.Create((true, 1)));
            Publisher.Publish(SignalWithValueTupleParameter.Create((false, 2)));

            mock.Received(1).OnNext(SignalWithValueTupleParameter.Create((true, 1)));
            mock.DidNotReceive().OnNext(SignalWithValueTupleParameter.Create((false, 2)));
        }
    }
}