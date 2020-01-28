using Zenject;

namespace SignalHandler.Application.Extension
{
    public static class DiContainerExtension
    {
        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder ProclaimSignal<TSignal>(this DiContainer container) where TSignal : ISignal
        {
            return container.DeclareSignal<TSignal>();
        }
    }
}