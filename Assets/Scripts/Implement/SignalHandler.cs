using System;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace SignalHandler
{
    [UsedImplicitly]
    internal class SignalHandler : ISignalReceiver, ISignalPublisher
    {
        internal SignalHandler(SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        private SignalBus SignalBus { get; }

        IObservable<TSignal> ISignalReceiver.Receive<TSignal>()
        {
            return SignalBus.GetStream<TSignal>();
        }

        IObservable<TSignal> ISignalReceiver.Receive<TSignal>(TSignal signal)
        {
            return SignalBus.GetStream<TSignal>().Where(x => x.Equals(signal));
        }

        IObservable<TSignal> ISignalReceiver.ReceiveWithParameter<TSignal, TParameter>()
        {
            return SignalBus.GetStream<TSignal>();
        }

        IObservable<TSignal> ISignalReceiver.ReceiveWithParameter<TSignal, TParameter>(TParameter parameter)
        {
            return SignalBus.GetStream<TSignal>().Where(x => x.Parameter.Equals(parameter));
        }

        IObservable<TSignal> ISignalReceiver.ReceiveWithParameter<TSignal, TParameter>(TSignal signal)
        {
            return SignalBus.GetStream<TSignal>().Where(x => x.Equals(signal));
        }

        void ISignalPublisher.Publish<TSignal>(TSignal signal)
        {
            SignalBus.Fire(signal);
        }
    }
}

