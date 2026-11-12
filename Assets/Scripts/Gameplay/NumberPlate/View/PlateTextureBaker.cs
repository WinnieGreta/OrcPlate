using UnityEngine;
using Zenject;

namespace Gameplay.NumberPlate.View
{
    public class PlateTextureBaker
    {
        [Inject] private Camera _plateBakeCamera;
        [Inject] private RectTransform _canvasRect;

        public RenderTexture BakePlateTexture(int width, int height)
        {
            RenderTexture plateTexture = new RenderTexture(width, height, 24);
            _canvasRect.sizeDelta = new Vector2(width, height);
            _plateBakeCamera.orthographicSize = height / 2f;
            _plateBakeCamera.targetTexture = plateTexture;
            _plateBakeCamera.ResetAspect();
            Canvas.ForceUpdateCanvases();
            _plateBakeCamera.Render();
            _plateBakeCamera.targetTexture = null;
            return plateTexture;
        }

    }
}