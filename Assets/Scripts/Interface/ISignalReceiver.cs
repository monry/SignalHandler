using System;

namespace SignalHandler
{
    public interface ISignalReceiver<TSignal> where TSignal : ISignal
    {
        IObservable<TSignal> Receive();
        IObservable<TSignal> Receive(TSignal signal);
    }

    public interface ISignalReceiver<TSignal, in TParameter> where TSignal : ISignal<TParameter>
    {
        IObservable<TSignal> Receive();
        IObservable<TSignal> Receive(TSignal signal);
        IObservable<TSignal> Receive(TParameter parameter);
    }
}