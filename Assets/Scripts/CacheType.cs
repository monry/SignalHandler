using System;
using UniRx;

namespace SignalHandler
{
    public enum CacheType
    {
        None   = 0,
        Latest = 1,
        All    = 2,
    }

    internal static class CacheTypeExtension
    {
        internal static ISubject<TSignal> AsReplaySubject<TSignal>(this CacheType cacheType)
        {
            switch (cacheType)
            {
                case CacheType.None:
                    return new ReplaySubject<TSignal>(0);
                case CacheType.Latest:
                    return new ReplaySubject<TSignal>(1);
                case CacheType.All:
                    return new ReplaySubject<TSignal>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
            }
        }
    }
}

namespace SignalHandler.Application.Master
{
    [Obsolete("Use SignalHandler.CacheType instead of this enum.")]
    public enum CacheType
    {
        None   = SignalHandler.CacheType.None,
        Latest = SignalHandler.CacheType.Latest,
        All    = SignalHandler.CacheType.All,
    }

    internal static class CacheTypeExtension
    {
        [Obsolete("Use SignalHandler.CacheTypeExtension.AsReplaySubject<TSignal>(this CacheType cacheType) instead of this method.")]
        internal static ISubject<TSignal> AsReplaySubject<TSignal>(this CacheType cacheType)
        {
            switch (cacheType)
            {
                case CacheType.None:
                    return new ReplaySubject<TSignal>(0);
                case CacheType.Latest:
                    return new ReplaySubject<TSignal>(1);
                case CacheType.All:
                    return new ReplaySubject<TSignal>();
                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
            }
        }
    }
}