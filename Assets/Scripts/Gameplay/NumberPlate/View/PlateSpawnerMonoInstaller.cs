using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.NumberPlate.View
{
    public class PlateSpawnerMonoInstaller: MonoInstaller
    {
        [SerializeField] private Camera _plateBakeCamera; 
        [SerializeField] private RectTransform _canvasRect;
        
        // TEMP for testing
        [SerializeField] private RawImage _image;
        [SerializeField] private Button _plateSpawnButton;

        public override void InstallBindings()
        {
            Container.BindInstance(_plateBakeCamera).AsSingle();
            Container.BindInstance(_canvasRect).AsSingle();
            
            Container.BindInstance(_image).AsSingle();
            Container.BindInstance(_plateSpawnButton).AsSingle();

            Container.BindInterfacesAndSelfTo<PlateTextureBaker>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlateSpawner>().AsSingle();
        }
    }
}