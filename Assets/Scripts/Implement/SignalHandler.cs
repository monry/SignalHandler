﻿using System;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace SignalHandler
{
    [UsedImplicitly]
    public class SignalHandler<TSignal> : ISignalPublisher<TSignal>, ISignalReceiver<TSignal>
        where TSignal : ISignal
    {
        internal SignalHandler(SignalBus signalBus, CacheType cacheType = CacheType.None)
        {
            SignalBus = signalBus;
            Subject = cacheType.AsReplaySubject<TSignal>();
            SignalBus.Subscribe<TSignal>(OnReceived);
        }

        private SignalBus SignalBus { get; }
        protected ISubject<TSignal> Subject { get; }

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

        public static void InstallSignal(DiContainer container)
        {
            if (!container.HasBinding<SignalBus>())
            {
                SignalBusInstaller.Install(container);
            }

            container
                .Bind(typeof(ISignalPublisher<TSignal>), typeof(ISignalReceiver<TSignal>))
                .To<SignalHandler<TSignal>>()
                .AsCached();

            container.DeclareSignal<TSignal>();
        }
    }

    [UsedImplicitly]
    public class SignalHandler<TSignal, TParameter> : SignalHandler<TSignal>, ISignalPublisher<TSignal, TParameter>, ISignalReceiver<TSignal, TParameter>
        where TSignal : ISignal<TParameter>
    {
        internal SignalHandler(SignalBus signalBus) : base(signalBus)
        {
        }

        IObservable<TSignal> ISignalReceiver<TSignal, TParameter>.Receive(TParameter parameter)
        {
            return Subject.Where(x => x.Parameter.Equals(parameter));
        }
    }
}

