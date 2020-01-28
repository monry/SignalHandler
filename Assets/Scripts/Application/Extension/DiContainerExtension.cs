using JetBrains.Annotations;
using Zenject;

namespace SignalHandler.Application.Extension
{
    [PublicAPI]
    public static class DiContainerExtension
    {
        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignalWithHandler<TSignal>(this DiContainer container) where TSignal : ISignal
        {
            if (!container.HasBinding<SignalBus>())
            {
                container.Bind<SignalBus>().AsCached();
            }

            container.BindInterfacesTo<SignalHandler<TSignal>>().AsCached();
            return container.DeclareSignal<TSignal>();
        }

        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignalWithHandler<TSignal, TParameter>(this DiContainer container) where TSignal : ISignal<TParameter>
        {
            if (!container.HasBinding<SignalBus>())
            {
                container.Bind<SignalBus>().AsCached();
            }

            container.BindInterfacesTo<SignalHandler<TSignal, TParameter>>().AsCached();
            return container.DeclareSignal<TSignal>();
        }
    }
}