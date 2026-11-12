using System;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.NumberPlate.View
{
    public class PlateSpawner: IInitializable, IDisposable
    {
        [Inject] private PlateTextureBaker _baker;
        [Inject] private Button _plateSpawnButton;
        [Inject] private SubPlateView.Factory _factory;

        public void Initialize()
        {
            _plateSpawnButton.onClick.AddListener(Spawn);
        }

        public void Dispose()
        {
            _plateSpawnButton.onClick.RemoveListener(Spawn);
        }

        private void Spawn()
        {
            var texture = _baker.BakePlateTexture(512, 128);
            var view = _factory.Create();
            view.Show(texture);
        }
    }
}