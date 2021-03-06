﻿using System;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace SignalHandler.UseCase
{
    [UsedImplicitly]
    internal class SignalHandler<TSignal> : ISignalPublisher<TSignal>, ISignalReceiver<TSignal>
        where TSignal : class, ISignal
    {
        [Inject]
        internal SignalHandler(SignalBus signalBus, CacheType cacheType = CacheType.None)
        {
            SignalBus = signalBus;
            Subject = cacheType.AsReplaySubject<TSignal>();
            SignalBus.Subscribe<TSignal>(OnReceived);
        }

        private SignalBus SignalBus { get; }
        private ISubject<TSignal> Subject { get; }

        void ISignalPublisher<TSignal>.Publish(TSignal signal)
        {
            SignalBus.Fire(signal);
        }

        IObservable<TSignal> ISignalReceiver<TSignal>.Receive(TSignal signal)
        {
            return Equals(signal, default) ? Subject : Subject.Where(x => x.Equals(signal));
        }

        // ReSharper disable once InvertIf
        private void OnReceived(TSignal signal)
        {
            Subject.OnNext(signal);

            if (signal.IsTerminator)
            {
                SignalBus.Unsubscribe<TSignal>(OnReceived);
                Subject.OnCompleted();
            }
        }
    }
}

