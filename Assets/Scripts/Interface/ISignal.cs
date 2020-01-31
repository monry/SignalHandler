using System;

namespace SignalHandler
{
    public interface ISignalTerminator
    {
        bool IsTerminator { get; }
    }

    public interface ISignal : IEquatable<ISignal>, ISignalTerminator
    {
    }

    public interface ISignal<TParameter> : ISignal, IEquatable<ISignal<TParameter>>
    {
        TParameter Parameter { get; }
    }
}