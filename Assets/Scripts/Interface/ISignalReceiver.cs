using System;

namespace SignalHandler
{
    public interface ISignalReceiver
    {
        IObservable<TSignal> Receive<TSignal>() where TSignal : ISignal;
        IObservable<TSignal> Receive<TSignal>(TSignal signal) where TSignal : ISignal;
        IObservable<TSignal> ReceiveWithParameter<TSignal, TParameter>() where TSignal : ISignal<TParameter>;
        IObservable<TSignal> ReceiveWithParameter<TSignal, TParameter>(TParameter parameter) where TSignal : ISignal<TParameter>;
        IObservable<TSignal> ReceiveWithParameter<TSignal, TParameter>(TSignal signal) where TSignal : ISignal<TParameter>;
    }
}