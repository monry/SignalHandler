using System.Reflection;
using SignalHandler.Application.Installer;
using SignalHandler.Application.Interface;
using SignalHandler.Application.Signal;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace SignalHandler
{
    public class ThrowSignal : SignalBase<ThrowSignal>, ISignalTerminator
    {
        bool ISignalTerminator.IsTerminator { get; } = true;
    }

    public class Subscriptionが無い場合に例外 : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<ThrowSignal> Publisher { get; }

        private ILogger MockLogger { get; } = Substitute.For<ILogger>();

        [SetUp]
        public void Install()
        {
            SignalHandlerInstaller<ThrowSignal>.Install(Container, signalMissingHandlerResponses: SignalMissingHandlerResponses.Throw);

            // 無理矢理 UnityEngine.Debug が用いる Logger を上書き
            typeof(Debug).GetField("s_Logger", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, MockLogger);

            Container.Inject(this);
        }

        [Test]
        public void A_Subscriptionが無い場合に例外()
        {
            // この Publish により Subject が OnCompleted するので Subscription が無くなる
            Publisher.Publish(ThrowSignal.Create());

            Assert.That(() => Publisher.Publish(ThrowSignal.Create()), Throws.TypeOf<ZenjectException>());
            Assert.That(() => Publisher.Publish(ThrowSignal.Create()), Throws.TypeOf<ZenjectException>());

            MockLogger.DidNotReceive().Log(LogType.Warning, Arg.Any<string>());
        }
    }
}