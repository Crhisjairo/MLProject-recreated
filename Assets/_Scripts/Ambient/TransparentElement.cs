using System;
using _Scripts.Enums;
using UnityEngine;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TransparentElement : MonoBehaviour
    {
        [SerializeField] private float fadeSpeed = 0.2f;
        [SerializeField] private float fadeAmount = 0.5f;

        [SerializeField] private string playerSortingLayer = "GameElements";
        [SerializeField] private int sortOrderToForeground = 9;

        private int _entitiesOnCollider;
        
        private SpriteRenderer _spriteRenderer;
        private string _defaultSortingLayerName;
        private int _defaultSortOrder;

        private void Awake()
        {
            SetComponents();
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
                var finalColor = _spriteRenderer.color;
                finalColor.a -= fadeAmount;

                LeanTween.color(_spriteRenderer.gameObject, finalColor, fadeSpeed);
        
                _spriteRenderer.sortingLayerName = playerSortingLayer.ToString();
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
                var startColor = _spriteRenderer.color;
                startColor.a += fadeAmount;
        
                LeanTween.color(_spriteRenderer.gameObject, startColor, fadeSpeed);
        
                _spriteRenderer.sortingLayerName = _defaultSortingLayerName;
                _spriteRenderer.sortingOrder = _defaultSortOrder;

            }
        
        }
    }
}