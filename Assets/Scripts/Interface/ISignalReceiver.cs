using System;

namespace SignalHandler
{
    public interface ISignalReceiver<TSignal> where TSignal : ISignal
    {
        IObservable<TSignal> Receive(TSignal signal = default);
    }

    public interface ISignalReceiver<TSignal, in TParameter> : ISignalReceiver<TSignal> where TSignal : ISignal<TParameter>
    {
        IObservable<TSignal> Receive(TParameter parameter);
    }
}