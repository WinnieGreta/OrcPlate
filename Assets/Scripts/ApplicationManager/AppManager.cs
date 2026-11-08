using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using Zenject;

namespace ApplicationManager
{ 
    public class AppManager : IApplicationManager, IAppInitializedCallback
    {
        [Inject] private SceneLoadingManager _sceneLoadingManager;

        public void OnAppInitialized()
        {
            LoadGameScene("Scene1");
        }

        public void LoadGameScene(string name)
        {
            _sceneLoadingManager.LoadScene(name).Forget();
        }

        public void OnAppExit()
        {
            Application.Quit();
        }

    }
    
}
