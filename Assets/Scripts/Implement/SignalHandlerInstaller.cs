using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Zenject;

namespace SignalHandler
{
    [UsedImplicitly]
    public class SignalHandlerInstaller<TSignal> : Installer<object, CacheType, SignalMissingHandlerResponses, SignalHandlerInstaller<TSignal>>
        where TSignal : ISignal
    {
        [Inject]
        public SignalHandlerInstaller(object identifier, CacheType cacheType, SignalMissingHandlerResponses signalMissingHandlerResponses)
        {
            Identifier = identifier;
            CacheType = cacheType;
            SignalMissingHandlerResponses = signalMissingHandlerResponses;
        }

        private object Identifier { get; }
        private CacheType CacheType { get; }
        private SignalMissingHandlerResponses SignalMissingHandlerResponses { get; }

        public override void InstallBindings()
        {
            if (!Container.HasBinding<SignalBus>())
            {
                SignalBusInstaller.Install(Container);
            }

            ConcreteBinderNonGeneric CreateBinder()
            {
                var binder = Container.Bind(typeof(ISignalPublisher<TSignal>), typeof(ISignalReceiver<TSignal>));
                return Identifier == default ? binder : binder.WithId(Identifier);
            }

            CreateBinder()
                .To<SignalHandler<TSignal>>()
                .AsCached()
                .WithArguments(CacheType)
                ;

            // ReSharper disable once InvertIf
            if (!SignalDeclarationStore.HasDeclaration<TSignal>(Container))
            {
                var declaredSignal = Container.DeclareSignal<TSignal>();
                switch (SignalMissingHandlerResponses)
                {
                    case SignalMissingHandlerResponses.Ignore:
                        declaredSignal.OptionalSubscriber();
                        break;
                    case SignalMissingHandlerResponses.Throw:
                        declaredSignal.RequireSubscriber();
                        break;
                    case SignalMissingHandlerResponses.Warn:
                        declaredSignal.OptionalSubscriberWithWarning();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(SignalMissingHandlerResponses), SignalMissingHandlerResponses, null);
                }
                SignalDeclarationStore.AddDeclaration<TSignal>(Container);
            }
        }

        // Override method to specify default arguments
        public new static void Install(DiContainer container, object identifier = default, CacheType cacheType = CacheType.None, SignalMissingHandlerResponses signalMissingHandlerResponses = SignalMissingHandlerResponses.Warn)
        {
            Installer<object, CacheType, SignalMissingHandlerResponses, SignalHandlerInstaller<TSignal>>.Install(container, identifier, cacheType, signalMissingHandlerResponses);
        }
    }

    internal static class SignalDeclarationStore
    {
        private static IDictionary<DiContainer, IList<Type>> DeclarationMap { get; } = new Dictionary<DiContainer, IList<Type>>();

        internal static bool HasDeclaration<TSignal>(DiContainer container)
        {
            return DeclarationMap.ContainsKey(container) && DeclarationMap[container].Contains(typeof(TSignal));
        }

        internal static void AddDeclaration<TSignal>(DiContainer container)
        {
            if (!DeclarationMap.ContainsKey(container))
            {
                DeclarationMap[container] = new List<Type>();
            }

            DeclarationMap[container].Add(typeof(TSignal));
        }
    }
}