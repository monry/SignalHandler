using System;
using SignalHandler.Application.Installer;
using SignalHandler.Application.Interface;
using SignalHandler.Application.Signal;
using NSubstitute;
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
            SignalHandlerInstaller<SignalWithStructParameter>.Install(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(SignalWithStructParameter.Create());
            Publisher.Publish(SignalWithStructParameter.Create());

            mock.Received(2).OnNext(Arg.Any<SignalWithStructParameter>());
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            var signalForPublish = SignalWithStructParameter.Create();
            var signalForReceive = SignalWithStructParameter.Create();

            Assert.False(ReferenceEquals(signalForPublish, signalForReceive));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.Received().OnNext(signalForPublish);
        }

        [Test]
        public void C_継承型は通す()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(ExtendedSignalWithStructParameter.Create());

            mock.Received().OnNext(Arg.Any<SignalWithStructParameter>());
        }

        [Test]
        public void D_パラメータの内容が等しい場合は通す()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            var signalForPublish = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1));
            var signalForReceive = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.Received(1).OnNext(signalForPublish);
        }

        [Test]
        public void E_パラメータの内容が異なる場合は通さない()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            var signalForPublish = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1));
            var signalForReceive = SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(false, 2));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.DidNotReceive().OnNext(signalForPublish);
        }

        [Test]
        public void F_送信したパラメータを受け取れる()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);
            Receiver
                .Receive()
                .Subscribe(
                    signal =>
                    {
                        Assert.That(signal.Parameter.BoolValue, Is.True);
                        Assert.That(signal.Parameter.IntValue, Is.EqualTo(1));
                    }
                );

            Publisher.Publish(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1)));
            Publisher.Publish(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1)));

            mock.Received(2).OnNext(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1)));
        }

        [Test]
        public void G_パラメータでフィルタできる()
        {
            var mock = Substitute.For<IObserver<SignalWithStructParameter>>();

            Receiver.Receive(new SignalWithStructParameter.StructParameter(true, 1)).Subscribe(mock.OnNext);

            Publisher.Publish(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1)));
            Publisher.Publish(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(false, 2)));

            mock.Received(1).OnNext(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(true, 1)));
            mock.DidNotReceive().OnNext(SignalWithStructParameter.Create(new SignalWithStructParameter.StructParameter(false, 2)));
        }
    }
}