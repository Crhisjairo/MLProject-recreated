using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class CoinsUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinsAmountText;
        [SerializeField] private Animator animator;

        public float animationTime = 0.5f, onScreenTime = 6f;

        public bool ShowOnAwake = true;

        private Vector3 _startAnimOffset = new Vector3(240, 0, 0);

        private Vector2 _startAnimPosition, _endAnimPosition;

        private bool _isPlayingAnimator = false;

        private Coroutine _startAnimationCoroutine;
        
        private void Start()
        {
            var position = transform.position;
            
            _startAnimPosition = position - _startAnimOffset;
            _endAnimPosition = position;
            
            
            gameObject.transform.position = _startAnimPosition;

        }

        public void SetCoinsAmountTo(int amount)
        {
            if (!ShowOnAwake)
            {
                ShowOnAwake = !ShowOnAwake;
                return;
            }
            
            if (!_isPlayingAnimator)
            {
                _startAnimationCoroutine = StartCoroutine(StartAnimation());
            }
            else
            {
                StopCoroutine(_startAnimationCoroutine);
                _startAnimationCoroutine = StartCoroutine(StartAnimation());
            }
            
            
            coinsAmountText.SetText(amount.ToString());
        }

        private IEnumerator StartAnimation()
        {
            _isPlayingAnimator = true;
            animator.enabled = true;
            
            LeanTween.moveX(gameObject, _endAnimPosition.x, animationTime).setEaseInOutBack();
            
            yield return new WaitForSeconds(onScreenTime);
            
            LeanTween.moveX(gameObject, _startAnimPosition.x, animationTime).setEaseInOutBack();
            
            yield return new WaitForSeconds(animationTime);

            _isPlayingAnimator = false;
            animator.enabled = false;
        }
    }
}