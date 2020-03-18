using System;
using UniRx;

namespace SignalHandler.Application.Master
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