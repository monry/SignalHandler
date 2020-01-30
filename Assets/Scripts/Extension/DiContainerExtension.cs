using JetBrains.Annotations;
using Zenject;

namespace SignalHandler
{
    [PublicAPI]
    public static class DiContainerExtension
    {
        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignalWithHandler<TSignal>(this DiContainer container) where TSignal : ISignal
        {
            if (!container.HasBinding<SignalBus>())
            {
                SignalBusInstaller.Install(container);
            }

            container
                .Bind(typeof(ISignalPublisher<TSignal>), typeof(ISignalReceiver<TSignal>))
                .To<SignalHandler<TSignal>>()
                .AsCached();
            return container.DeclareSignal<TSignal>();
        }

        public static DeclareSignalIdRequireHandlerAsyncTickPriorityCopyBinder DeclareSignalWithHandler<TSignal, TParameter>(this DiContainer container) where TSignal : ISignal<TParameter>
        {
            if (!container.HasBinding<SignalBus>())
            {
                SignalBusInstaller.Install(container);
            }

            container
                .Bind(typeof(ISignalPublisher<TSignal>), typeof(ISignalReceiver<TSignal>))
                .To<SignalHandler<TSignal>>()
                .AsCached();
            return container.DeclareSignal<TSignal>();
        }
    }
}
