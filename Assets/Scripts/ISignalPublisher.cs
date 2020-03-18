using System;
using JetBrains.Annotations;

namespace SignalHandler
{
    [PublicAPI]
    public interface ISignalPublisher<in TSignal> where TSignal : ISignal
    {
        void Publish(TSignal signal);
    }
}
namespace SignalHandler.Application.Interface
{
    [PublicAPI]
    [Obsolete("Use SignalHandler.ISignalPublisher<TSignal> instead of this interface.")]
    public interface ISignalPublisher<in TSignal> : SignalHandler.ISignalPublisher<TSignal> where TSignal : SignalHandler.ISignal
    {
    }
}
