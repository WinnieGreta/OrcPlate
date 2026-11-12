using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.NumberPlate.View
{
    public class PlateSpawner: IInitializable, IDisposable
    {
        [Inject] private PlateTextureBaker _baker;
        [Inject] private RawImage _image;
        [Inject] private Button _plateSpawnButton;

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
            _image.texture = texture;
        }
    }
}