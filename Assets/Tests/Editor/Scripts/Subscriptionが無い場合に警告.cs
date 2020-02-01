using System.Reflection;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace SignalHandler
{
    public class WarnSignal : SignalBase<WarnSignal>, ISignalTerminator
    {
        bool ISignalTerminator.IsTerminator { get; } = true;
    }

    public class Subscriptionが無い場合に警告 : ZenjectUnitTestFixture
    {
        [Inject] private ISignalPublisher<WarnSignal> Publisher { get; }

        private ILogger MockLogger { get; } = Substitute.For<ILogger>();

        [SetUp]
        public void Install()
        {
            // ReSharper disable once RedundantArgumentDefaultValue 書き味を揃えるためにデフォルト引数を明示
            SignalHandlerInstaller<WarnSignal>.Install(Container, signalMissingHandlerResponses: SignalMissingHandlerResponses.Warn);

            // 無理矢理 UnityEngine.Debug が用いる Logger を上書き
            typeof(Debug).GetField("s_Logger", BindingFlags.Static | BindingFlags.NonPublic)?.SetValue(null, MockLogger);

            Container.Inject(this);
        }

        [Test]
        public void A_Subscriptionが無い場合に警告()
        {
            // この Publish により Subject が OnCompleted するので Subscription が無くなる
            Publisher.Publish(WarnSignal.Create());

            Publisher.Publish(WarnSignal.Create());
            Publisher.Publish(WarnSignal.Create());

            MockLogger.Received(2).Log(LogType.Warning, Arg.Any<string>());
        }
    }

}