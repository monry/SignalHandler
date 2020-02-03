using JetBrains.Annotations;

namespace SignalHandler
{
    [PublicAPI]
    public interface ISignalPublisher<in TSignal> where TSignal : ISignal
    {
        void Publish(TSignal signal);
    }
}

