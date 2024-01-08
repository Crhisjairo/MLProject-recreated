using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.UI.CharacterUI
{
    public class CharacterCanvas : MonoBehaviour
    {
        
        private Vector3 _startAnimOffset = new Vector3(0, -200, 0);
        
        private Vector2 _startAnimPosition, _endAnimPosition;
        
        public float animationTime = 1f, onScreenTime = 13f;

        public bool ShowOnAwake = true;
        
        private bool _isOnAnimation = false;
        
        private Coroutine _startAnimationCoroutine;
        
        private void Start()
        {
            var position = transform.position;
            
            _startAnimPosition = position - _startAnimOffset;
            _endAnimPosition = position;

            gameObject.transform.position = _startAnimPosition;

        }
        
        public void Show()
        {
            if (!ShowOnAwake)
            {
                ShowOnAwake = !ShowOnAwake;
                return;
            }
            
            if (!_isOnAnimation)
            {
                _startAnimationCoroutine = StartCoroutine(StartAnimation());
            }
            else
            {
                StopCoroutine(_startAnimationCoroutine);
                _startAnimationCoroutine = StartCoroutine(StartAnimation());
            }
        }
        
        private IEnumerator StartAnimation()
        {
            _isOnAnimation = true;
            
            LeanTween.moveY(gameObject, _endAnimPosition.y, animationTime).setEaseOutBack();
            
            yield return new WaitForSeconds(onScreenTime);
            
            LeanTween.moveY(gameObject, _startAnimPosition.y, animationTime).setEaseInBack();
            
            yield return new WaitForSeconds(animationTime);

            _isOnAnimation = false;
        }
        
    }
}