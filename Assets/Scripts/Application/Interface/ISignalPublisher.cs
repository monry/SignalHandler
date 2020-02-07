using JetBrains.Annotations;

namespace SignalHandler.Application.Interface
{
    [PublicAPI]
    public interface ISignalPublisher<in TSignal> where TSignal : ISignal
    {
        void Publish(TSignal signal);
    }
}

