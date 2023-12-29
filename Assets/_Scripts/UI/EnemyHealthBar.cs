using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        public bool showOnStart = false;
        
        public Size size = Size.Small;
        public UnityEvent onSizeChanged;

        public float fadeAnimationTime = 1;
        
        private float _sizeMultiplier = 0.4f;

        private Vector3 baseScale = new Vector3(1, 1, 1);
        
        private void Start()
        {
            SetHealthBarActive(showOnStart);
            
            SetSize(size);
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
                LeanTween.alphaCanvas(_canvasGroup, 1, fadeAnimationTime);
            }
            else
            {
                LeanTween.alphaCanvas(_canvasGroup, 0, fadeAnimationTime);
            }
        }

        private void SetSize(Size newSize)
        {
            this.size = newSize;
            
            if (size == Size.Small)
            {
                Vector3 scale = new Vector3(transform.localScale.x - _sizeMultiplier, transform.localScale.y - _sizeMultiplier, transform.localScale.z);
                transform.localScale = scale;
            } else
            {
                transform.localScale = baseScale;
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