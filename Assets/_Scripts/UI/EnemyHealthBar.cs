using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [FormerlySerializedAs("_canvasGroup")] [SerializeField] private CanvasGroup canvasGroup;
        public bool showOnStart;
        
        private Size _size = Size.Small;
        public UnityEvent onSizeChanged;

        public float fadeAnimationTime = 1;
        
        private float _sizeMultiplier = 0.4f;

        private readonly Vector3 _baseScale = new Vector3(1, 1, 1);
        
        private void Start()
        {
            SetHealthBarActive(showOnStart);
        }

        public void SetHealthSmallBarSize()
        {
            SetSize(Size.Small);
        }

        public void SetHealthBigBarSize()
        {
            SetSize(Size.Big);
        }

        public void SetHealthBarActive(bool isActive)
        {
            if (isActive)
            {
                LeanTween.alphaCanvas(canvasGroup, 1, fadeAnimationTime);
            }
            else
            {
                LeanTween.alphaCanvas(canvasGroup, 0, fadeAnimationTime);
            }
        }

        private void SetSize(Size newSize)
        {
            this._size = newSize;
            
            if (_size == Size.Small)
            {
                Vector3 scale = new Vector3(transform.localScale.x - _sizeMultiplier, transform.localScale.y - _sizeMultiplier, transform.localScale.z);
                transform.localScale = scale;
            } else
            {
                transform.localScale = _baseScale;
            }
            
            onSizeChanged?.Invoke();
        }
    }

    public enum Size
    {
        Small,
        Big
    }
}