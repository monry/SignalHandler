using System;

namespace SignalHandler
{
    public interface ISignal : IEquatable<ISignal>
    {
    }

    public interface ISignal<TParameter> : ISignal, IEquatable<ISignal<TParameter>>
    {
        TParameter Parameter { get; }
    }
}