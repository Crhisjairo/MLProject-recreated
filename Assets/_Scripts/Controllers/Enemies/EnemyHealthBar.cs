using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Controllers.Enemies
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        public bool showOnStart = false;
        
        public Size size = Size.Small;
        public UnityEvent onSizeChanged;

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
            // TODO: add a fade animation. 
            if (isActive)
            {
                _canvasGroup.alpha = 1;
            }
            else
            {
                _canvasGroup.alpha = 0;
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