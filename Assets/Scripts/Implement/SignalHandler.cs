using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UniRx;
using Zenject;

namespace SignalHandler
{
    [UsedImplicitly]
    public class SignalHandler<TSignal> : ISignalPublisher<TSignal>, ISignalReceiver<TSignal>
        where TSignal : ISignal
    {
        internal SignalHandler(SignalBus signalBus, CacheType cacheType = CacheType.None)
        {
            SignalBus = signalBus;
            Subject = cacheType.AsReplaySubject<TSignal>();
            SignalBus.Subscribe<TSignal>(OnReceived);
        }

        private SignalBus SignalBus { get; }
        private ISubject<TSignal> Subject { get; }

        void ISignalPublisher<TSignal>.Publish(TSignal signal)
        {
            SignalBus.Fire(signal);
        }

        IObservable<TSignal> ISignalReceiver<TSignal>.Receive(TSignal signal)
        {
            return Equals(signal, default) ? Subject : Subject.Where(x => x.Equals(signal));
        }

        // ReSharper disable once InvertIf
        private void OnReceived(TSignal signal)
        {
            Subject.OnNext(signal);

            if (signal.IsTerminator)
            {
                SignalBus.Unsubscribe<TSignal>(OnReceived);
                Subject.OnCompleted();
            }
        }

        public static void InstallSignal(DiContainer container, object identifier = default, CacheType cacheType = CacheType.None)
        {
            if (!container.HasBinding<SignalBus>())
            {
                SignalBusInstaller.Install(container);
            }

            ConcreteBinderNonGeneric CreateBinder()
            {
                var binder = container.Bind(typeof(ISignalPublisher<TSignal>), typeof(ISignalReceiver<TSignal>));
                return identifier == default ? binder : binder.WithId(identifier);
            }

            CreateBinder()
                .To<SignalHandler<TSignal>>()
                .AsCached()
                .WithArguments(cacheType)
            ;

            if (!SignalDeclarationStore.HasDeclaration<TSignal>(container))
            {
                container.DeclareSignal<TSignal>();
                SignalDeclarationStore.AddDeclaration<TSignal>(container);
            }
        }

        // ReSharper disable once UnusedMember.Local Avoid constructor stripping by IL2CPP
        private static void AOTWorkaround()
        {
            {
                var _ = new SignalHandler<TSignal>(default);
            }
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

