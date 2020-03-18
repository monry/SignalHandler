using System.Reflection;
using SignalHandler.Installer;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace SignalHandler
{
    public class IgnoreSignal : SignalBase<IgnoreSignal>, ISignalTerminator
    {
        bool ISignalTerminator.IsTerminator { get; } = true;
    }

    public class Subscriptionが無い場合も無視 : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<IgnoreSignal> Publisher { get; }

        private ILogger MockLogger { get; } = Substitute.For<ILogger>();

        [SetUp]
        public void Install()
        {
            SignalHandlerInstaller<IgnoreSignal>.Install(Container, signalMissingHandlerResponses: SignalMissingHandlerResponses.Ignore);

            // 無理矢理 UnityEngine.Debug が用いる Logger を上書き
            typeof(Debug).GetField("s_Logger", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, MockLogger);

            Container.Inject(this);
        }

        [Test]
        public void A_Subscriptionが無い場合も無視()
        {
            // この Publish により Subject が OnCompleted するので Subscription が無くなる
            Publisher.Publish(IgnoreSignal.Create());

            Publisher.Publish(IgnoreSignal.Create());
            Publisher.Publish(IgnoreSignal.Create());

            MockLogger.DidNotReceive().Log(LogType.Warning, Arg.Any<string>());
        }
    }
}