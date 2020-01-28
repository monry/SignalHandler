# Signal Handler

## Installation

```bash
upm add package dev.monry.signalhandler
```

Note: `upm` command is provided by [this repository](https://github.com/upm-packages/upm-cli).

## Usages

### Create Signal types

#### Type based signal

```csharp
using SignalHandler;

public class FooSignal : SignalBase<FooSignal>
{
}
```

#### Type based signal with parameters

```csharp
using SignalHandler;

public struct SomeParameter
{
    private bool boolParameter;
    private int intParameter;

    public SomeParameter(bool boolParameter, int intParameter)
    {
        this.boolParameter = boolParameter;
        this.intParameter = intParameter;
    }
}

public class BarSignal : SignalBase<BarSignal, SomeParameter>
{
}
```

#### ScriptableObject signals

Use `ScriptableObjectSignalBase<TSignal>` or `ScriptableObjectSignalBase<TSignal, TParameter>`.

### Declare signals in Zenject.Installer

```csharp
using Zenject;
using SignalHandler; // provides extension methods

public class SomeInstaller : MonoInstaller<SomeInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignalWithHandler<FooSignal>();
        Container.DeclareSignalWithHandler<BarSignal, SomeParameter>();
    }
}
```

### Publish signals

```csharp
using SignalHandler;
using Zenject;
using UnityEngine;

public class MyPublisherMonoBehaviour : MonoBehaviour
{
    [Inject] private ISignalPublisher<FooSignal> FooPublisher { get; }
    [Inject] private ISignalPublisher<BarSignal, SomeParameter> BarPublisher { get; }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);
        FooPublisher.Publish(FooSignal.Create());
        yield return new WaitForSeconds(1.0f);
        BarPublisher.Publish(BarSignal.Create(new SomeParameter(true, 1)));
    }
}
```

### Receive signals

```csharp
using SignalHandler;
using Zenject;
using UnityEngine;
using UniRx;

public class MyReceiverMonoBehaviour : MonoBehaviour
{
    [Inject] private ISignalReceiver<FooSignal> FooReceiver { get; }
    [Inject] private ISignalReceiver<BarSignal, SomeParameter> BarReceiver { get; }

    public void Start()
    {
        // Log after 1 second after `MyPublisherMonoBehaviour.Start()` invoked.
        FooReceiver.Receive().Subscribe(_ => Debug.Log("Receive FooSignal"));
        // Does not log because type does not match
        FooReceiver.Receive(ExtendsFooSignal.Create()).Subscribe(_ => Debug.Log("Will not invoke"));

        // Log after 2 second after `MyPublisherMonoBehaviour.Start()` invoked.
        BarReceiver.Receive().Subscribe(_ => Debug.Log("Receive BarSignal (normal)"));
        // Does not log because type does not match
        BarReceiver.Receive(ExtendsBarSignal.Create(default)).Subscribe(_ => Debug.Log("Will not invoke"));
        // Log after 2 second after `MyPublisherMonoBehaviour.Start()` invoked because signal does match.
        BarReceiver.Receive(BarSignal.Create(new SomeParameter(true, 1))).Subscribe(_ => Debug.Log("Receive BarSignal (signal matches)"));
        // Log after 2 second after `MyPublisherMonoBehaviour.Start()` invoked because parameter does match.
        BarReceiver.Receive(new SomeParameter(true, 1)).Subscribe(_ => Debug.Log("Receive BarSignal (parameter matches)"));
        // Does not log because parameter does not match
        BarReceiver.Receive(new SomeParameter(false, 0)).Subscribe(_ => Debug.Log("Will not invoke"));
    }

    private class ExtendsFooSignal : FooSignal
    {
    }

    private class ExtendsBarSignal : BarSignal
    {
    }
}
```
