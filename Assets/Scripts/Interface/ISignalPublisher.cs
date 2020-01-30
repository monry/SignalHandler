namespace SignalHandler
{
    public interface ISignalPublisher<in TSignal> where TSignal : ISignal
    {
        void Publish(TSignal signal);
    }

    public interface ISignalPublisher<in TSignal, in TParameter> : ISignalPublisher<TSignal> where TSignal : ISignal<TParameter>
    {
    }
}

