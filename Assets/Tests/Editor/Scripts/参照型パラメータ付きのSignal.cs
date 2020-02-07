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
            SignalHandlerInstaller<SignalWithClassParameter>.Install(Container);

            Container.Inject(this);
        }

        [Test]
        public void A_通常の送受信()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(SignalWithClassParameter.Create());
            Publisher.Publish(SignalWithClassParameter.Create());

            mock.Received(2).OnNext(Arg.Any<SignalWithClassParameter>());
        }

        [Test]
        public void B_異なるインスタンスでも通す()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            var signalForPublish = SignalWithClassParameter.Create();
            var signalForReceive = SignalWithClassParameter.Create();

            Assert.False(ReferenceEquals(signalForPublish, signalForReceive));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.Received().OnNext(signalForPublish);
        }

        [Test]
        public void C_継承型は通す()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            Receiver.Receive().Subscribe(mock.OnNext);

            Publisher.Publish(ExtendedSignalWithClassParameter.Create());

            mock.Received().OnNext(Arg.Any<SignalWithClassParameter>());
        }

        [Test]
        public void D_パラメータの内容が等しくても参照が異なる場合は通さない()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            var signalForPublish = SignalWithClassParameter.Create(new SignalWithClassParameter.ClassParameter(true, 1));
            var signalForReceive = SignalWithClassParameter.Create(new SignalWithClassParameter.ClassParameter(true, 1));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.DidNotReceive().OnNext(signalForPublish);
        }

        [Test]
        public void E_パラメータの内容が異なる場合は通さない()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            var signalForPublish = SignalWithClassParameter.Create(new SignalWithClassParameter.ClassParameter(true, 1));
            var signalForReceive = SignalWithClassParameter.Create(new SignalWithClassParameter.ClassParameter(false, 2));

            Receiver.Receive(signalForReceive).Subscribe(mock.OnNext);

            Publisher.Publish(signalForPublish);

            mock.DidNotReceive().OnNext(signalForPublish);
        }

        [Test]
        public void F_送信したパラメータを受け取れる()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            var parameter = new SignalWithClassParameter.ClassParameter(true, 1);

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

            Publisher.Publish(SignalWithClassParameter.Create(parameter));
            Publisher.Publish(SignalWithClassParameter.Create(parameter));

            mock.Received(2).OnNext(SignalWithClassParameter.Create(parameter));
        }

        [Test]
        public void G_パラメータでフィルタできる()
        {
            var mock = Substitute.For<IObserver<SignalWithClassParameter>>();

            var parameterPublishable = new SignalWithClassParameter.ClassParameter(true, 1);
            var parameterUnpublishable = new SignalWithClassParameter.ClassParameter(true, 1);

            Receiver.Receive(parameterPublishable).Subscribe(mock.OnNext);

            Publisher.Publish(SignalWithClassParameter.Create(parameterPublishable));
            Publisher.Publish(SignalWithClassParameter.Create(parameterUnpublishable));

            mock.Received(1).OnNext(SignalWithClassParameter.Create(parameterPublishable));
            mock.DidNotReceive().OnNext(SignalWithClassParameter.Create(parameterUnpublishable));
        }
    }
}