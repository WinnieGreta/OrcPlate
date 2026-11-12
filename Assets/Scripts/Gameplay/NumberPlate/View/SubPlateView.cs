using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gameplay.NumberPlate.View
{
    public class SubPlateView : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private RawImage _image;

        public void Show(Texture texture)
        {
            _rectTransform.sizeDelta = new Vector2(texture.width / 2f, texture.height / 2f);
            _image.texture = texture;
        }

        public class Factory : PlaceholderFactory<SubPlateView>
        {
            
        }
    }
}