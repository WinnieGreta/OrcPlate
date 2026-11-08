using Zenject;

namespace ApplicationManager
{
    public class AppManagerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AppManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SceneLoadingManager>().AsSingle();
            
        }
    }
}