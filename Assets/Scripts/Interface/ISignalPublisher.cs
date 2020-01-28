namespace SignalHandler
{
    public interface ISignalPublisher
    {
        void Publish<TSignal>(TSignal signal) where TSignal : ISignal;
    }
}

