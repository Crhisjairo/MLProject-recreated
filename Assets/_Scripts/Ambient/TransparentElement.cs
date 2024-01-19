using UnityEngine;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TransparentElement : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed = 0.2f;
        [SerializeField] private float fadeAmount = 0.5f;

        private Color _initialColor, _finalColor;
        
        [SerializeField] private string playerSortingLayer = "GameElements";
        [SerializeField] private int sortOrderToForeground = 9;

        private int _entitiesOnCollider;
        
        private SpriteRenderer _spriteRenderer;
        private string _defaultSortingLayerName;
        private int _defaultSortOrder;

        private void Awake()
        {
            SetComponents();

            _initialColor = _finalColor = _spriteRenderer.color;
            
            _finalColor.a -= fadeAmount;
        }

        private void SetComponents()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            _defaultSortingLayerName = _spriteRenderer.sortingLayerName;
            _defaultSortOrder = _spriteRenderer.sortingOrder;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.isTrigger)
                return;
            
            // Only change alpha value when the first entity collides.
            if (_entitiesOnCollider == 0)
            {

                LeanTween.color(_spriteRenderer.gameObject, _finalColor, fadeSpeed);
        
                _spriteRenderer.sortingLayerName = playerSortingLayer;
                _spriteRenderer.sortingOrder = sortOrderToForeground;
            }

            _entitiesOnCollider++;
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if(col.isTrigger)
                return;

            _entitiesOnCollider--;
            
            if (_entitiesOnCollider == 0)
            {
                LeanTween.color(_spriteRenderer.gameObject, _initialColor, fadeSpeed);
        
                _spriteRenderer.sortingLayerName = _defaultSortingLayerName;
                _spriteRenderer.sortingOrder = _defaultSortOrder;
            }
        }
    }
}