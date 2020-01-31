using System;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace SignalHandler
{
    [UsedImplicitly]
    public class SignalHandler<TSignal> : ISignalPublisher<TSignal>, ISignalReceiver<TSignal>
        where TSignal : ISignal
    {
        internal SignalHandler(SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        private SignalBus SignalBus { get; }

        void ISignalPublisher<TSignal>.Publish(TSignal signal)
        {
            SignalBus.Fire(signal);
        }

        IObservable<TSignal> ISignalReceiver<TSignal>.Receive()
        {
            return SignalBus.GetStream<TSignal>();
        }

        IObservable<TSignal> ISignalReceiver<TSignal>.Receive(TSignal signal)
        {
            return SignalBus.GetStream<TSignal>().Where(x => x.Equals(signal));
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
    public class SignalHandler<TSignal, TParameter> : ISignalPublisher<TSignal, TParameter>, ISignalReceiver<TSignal, TParameter>
        where TSignal : ISignal<TParameter>
    {
        internal SignalHandler(SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        private SignalBus SignalBus { get; }

        void ISignalPublisher<TSignal, TParameter>.Publish(TSignal signal)
        {
            SignalBus.Fire(signal);
        }

        IObservable<TSignal> ISignalReceiver<TSignal, TParameter>.Receive()
        {
            return SignalBus.GetStream<TSignal>();
        }

        IObservable<TSignal> ISignalReceiver<TSignal, TParameter>.Receive(TSignal signal)
        {
            return SignalBus.GetStream<TSignal>().Where(x => x.Equals(signal));
        }

        IObservable<TSignal> ISignalReceiver<TSignal, TParameter>.Receive(TParameter parameter)
        {
            return SignalBus.GetStream<TSignal>().Where(x => x.Parameter.Equals(parameter));
        }

        public static void InstallSignal(DiContainer container)
        {
            if (!container.HasBinding<SignalBus>())
            {
                SignalBusInstaller.Install(container);
            }

            container
                .Bind(typeof(ISignalPublisher<TSignal, TParameter>), typeof(ISignalReceiver<TSignal, TParameter>))
                .To<SignalHandler<TSignal, TParameter>>()
                .AsCached();

            container.DeclareSignal<TSignal>();
        }
    }
}

