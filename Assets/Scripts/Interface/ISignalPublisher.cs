namespace SignalHandler
{
    public interface ISignalPublisher<in TSignal> where TSignal : ISignal
    {
        void Publish(TSignal signal);
    }

    public interface ISignalPublisher<in TSignal, TParameter> where TSignal : ISignal<TParameter>
    {
        void Publish(TSignal signal);
    }
}

