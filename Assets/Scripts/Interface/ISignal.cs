using System;
using JetBrains.Annotations;

namespace SignalHandler
{
    [PublicAPI]
    public interface ISignalTerminator
    {
        bool IsTerminator { get; }
    }

    [PublicAPI]
    public interface ISignal : IEquatable<ISignal>, ISignalTerminator
    {
    }

    [PublicAPI]
    public interface ISignal<TParameter> : ISignal, IEquatable<ISignal<TParameter>>
    {
        TParameter Parameter { get; }
    }
}