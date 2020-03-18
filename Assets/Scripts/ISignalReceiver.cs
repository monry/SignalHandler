using System;
using JetBrains.Annotations;
using UniRx;

namespace SignalHandler
{
    [PublicAPI]
    public interface ISignalReceiver<TSignal> where TSignal : ISignal
    {
        IObservable<TSignal> Receive(TSignal signal = default);
    }

    [PublicAPI]
    public static class SignalReceiverExtension
    {
        public static IObservable<TSignal> Receive<TSignal, TParameter>(this ISignalReceiver<TSignal> signalReceiver, TParameter parameter)
            where TSignal : ISignal<TParameter>
        {
            return signalReceiver.Receive().Where(signal => Equals(signal.Parameter, parameter));
        }
    }
}

namespace SignalHandler.Application.Interface
{
    [PublicAPI]
    [Obsolete("Use SignalHandler.ISignalReceiver<TSignal> instead of this interface.")]
    public interface ISignalReceiver<TSignal> : SignalHandler.ISignalReceiver<TSignal> where TSignal : SignalHandler.ISignal
    {
    }

    [PublicAPI]
    public static class SignalReceiverExtension
    {
        [Obsolete("Use SignalReceiverExtension.Receive<TSignal, TParameter>(this ISignalReceiver<TSignal> signalReceiver, TParameter parameter) instead of this method.")]
        public static IObservable<TSignal> Receive<TSignal, TParameter>(this ISignalReceiver<TSignal> signalReceiver, TParameter parameter)
            where TSignal : SignalHandler.ISignal<TParameter>
        {
            return ((SignalHandler.ISignalReceiver<TSignal>) signalReceiver).Receive(parameter);
        }
    }
}