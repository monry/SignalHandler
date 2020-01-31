using System;
using UniRx;

namespace SignalHandler
{
    public interface ISignalReceiver<TSignal> where TSignal : ISignal
    {
        IObservable<TSignal> Receive(TSignal signal = default);
    }

    public static class SignalReceiverExtension
    {
        public static IObservable<TSignal> Receive<TSignal, TParameter>(this ISignalReceiver<TSignal> signalReceiver, TParameter parameter)
            where TSignal : ISignal<TParameter>
        {
            return signalReceiver.Receive().Where(signal => Equals(signal.Parameter, parameter));
        }
    }
}