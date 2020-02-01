using System;
using NSubstitute;
using NUnit.Framework;
using Zenject;
using UniRx;

namespace SignalHandler
{
    public class CacheSignal : SignalBase<CacheSignal, int>
    {
    }

    public class Cacheを制御するSignal : ZenjectUnitTestFixture
    {
        [Inject(Id = CacheType.None)] private ISignalPublisher<CacheSignal> PublisherForCacheNone { get; }
        [Inject(Id = CacheType.None)] private ISignalReceiver<CacheSignal> ReceiverForCacheNone { get; }
        [Inject(Id = CacheType.Latest)] private ISignalPublisher<CacheSignal> PublisherForCacheLatest { get; }
        [Inject(Id = CacheType.Latest)] private ISignalReceiver<CacheSignal> ReceiverForCacheLatest { get; }
        [Inject(Id = CacheType.All)] private ISignalPublisher<CacheSignal> PublisherForCacheAll { get; }
        [Inject(Id = CacheType.All)] private ISignalReceiver<CacheSignal> ReceiverForCacheAll { get; }

        [SetUp]
        public void Install()
        {
            // ReSharper disable once RedundantArgumentDefaultValue 書き味を揃えるためにデフォルト引数を明示
            SignalHandler<CacheSignal>.InstallSignal(Container, CacheType.None, CacheType.None);
            SignalHandler<CacheSignal>.InstallSignal(Container, CacheType.Latest, CacheType.Latest);
            SignalHandler<CacheSignal>.InstallSignal(Container, CacheType.All, CacheType.All);

            Container.Inject(this);
        }

        [Test]
        public void A_CacheTypeNoneの送受信()
        {
            var mock = Substitute.For<IObserver<CacheSignal>>();

            ReceiverForCacheNone.Receive().Subscribe(mock.OnNext);

            PublisherForCacheNone.Publish(CacheSignal.Create(1));
            PublisherForCacheNone.Publish(CacheSignal.Create(2));

            mock.Received(2).OnNext(Arg.Any<CacheSignal>());

            mock.ClearReceivedCalls();

            ReceiverForCacheNone.Receive().Subscribe(mock.OnNext);

            mock.DidNotReceive().OnNext(Arg.Any<CacheSignal>());
        }

        [Test]
        public void B_CacheTypeLatestの送受信()
        {
            var mock = Substitute.For<IObserver<CacheSignal>>();

            ReceiverForCacheLatest.Receive().Subscribe(mock.OnNext);

            PublisherForCacheLatest.Publish(CacheSignal.Create(1));
            PublisherForCacheLatest.Publish(CacheSignal.Create(2));

            mock.Received(2).OnNext(Arg.Any<CacheSignal>());
            mock.Received(1).OnNext(CacheSignal.Create(1));
            mock.Received(1).OnNext(CacheSignal.Create(2));

            mock.ClearReceivedCalls();

            ReceiverForCacheLatest.Receive().Subscribe(mock.OnNext);

            mock.Received(1).OnNext(Arg.Any<CacheSignal>());
            mock.Received(1).OnNext(CacheSignal.Create(2));
        }

        [Test]
        public void C_CacheTypeAllの送受信()
        {
            var mock = Substitute.For<IObserver<CacheSignal>>();

            ReceiverForCacheAll.Receive().Subscribe(mock.OnNext);

            PublisherForCacheAll.Publish(CacheSignal.Create(1));
            PublisherForCacheAll.Publish(CacheSignal.Create(2));

            mock.Received(2).OnNext(Arg.Any<CacheSignal>());
            mock.Received(1).OnNext(CacheSignal.Create(1));
            mock.Received(1).OnNext(CacheSignal.Create(2));

            mock.ClearReceivedCalls();

            ReceiverForCacheAll.Receive().Subscribe(mock.OnNext);

            mock.Received(2).OnNext(Arg.Any<CacheSignal>());
            mock.Received(1).OnNext(CacheSignal.Create(1));
            mock.Received(1).OnNext(CacheSignal.Create(2));
        }
    }
}