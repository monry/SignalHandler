using Zenject;

namespace SignalHandler.Application.Installer
{
    public class SignalHandlerInstaller : MonoInstaller<SignalHandlerInstaller>
    {
        public override void InstallBindings()
        {
            if (!Container.HasBinding<SignalBus>())
            {
                Container.Bind<SignalBus>().AsCached();
            }

            Container
                .BindInterfacesTo<SignalHandler>()
                .AsCached();
        }
    }
}