using System;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace SignalHandler
{
    [UsedImplicitly]
    internal class SignalHandler<TSignal> : ISignalPublisher<TSignal>, ISignalReceiver<TSignal>
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
    }

    [UsedImplicitly]
    internal class SignalHandler<TSignal, TParameter> : ISignalPublisher<TSignal, TParameter>, ISignalReceiver<TSignal, TParameter>
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
    }
}

