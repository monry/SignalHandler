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

namespace SignalHandler.Application.Interface
{
    [PublicAPI]
    [Obsolete("Use SignalHandler.ISignalTerminator instead of this interface.")]
    public interface ISignalTerminator : SignalHandler.ISignalTerminator
    {
    }

    [PublicAPI]
    [Obsolete("Use SignalHandler.ISignal instead of this interface.")]
    public interface ISignal : SignalHandler.ISignal
    {
    }

    [PublicAPI]
    [Obsolete("Use SignalHandler.ISignal<TParameter> instead of this interface.")]
    public interface ISignal<TParameter> : SignalHandler.ISignal<TParameter>
    {
    }
}