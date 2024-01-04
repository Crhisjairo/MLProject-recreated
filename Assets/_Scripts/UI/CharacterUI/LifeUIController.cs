using System;
using System.Collections.Generic;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.UI
{
    public class LifeUIController : MonoBehaviour
    {
        [SerializeField] private Sprite emptyHeartSprite, filledHeartSprite;
        [SerializeField] private float heartAnimationSpeed = 0.5f;
        [SerializeField] private Vector3 heartAnimationSize = new Vector3(1.2f, 1.2f, 1);

        private Vector3 _defaultHeartScale;
        private List<Image> _heartImages;
        private GameObject _currentHeart;
        
        private void Awake()
        {
            _heartImages = new List<Image>();

            SetHeartSpriteRenderers();
        }

        private void SetHeartSpriteRenderers()
        {   
            foreach (Transform child in transform)
            {
                _heartImages.Add(child.gameObject.GetComponent<Image>());
            }

            _defaultHeartScale = _heartImages[0].transform.localScale;
        }

        public void SetMaxHeartsTo(int amount)
        {
            if (amount > _heartImages.Count)
            {
                Debug.LogError(ConsoleMessages.MaxLifeMoreThanMaxHeartsOnUI);
                return;
            }
            
            for (int i = 0; i < amount; i++)
            {
                _heartImages[i].enabled = true;
            }
            
            for (int i = amount; i < _heartImages.Count; i++)
            {
                _heartImages[i].enabled = false;
            }
            
            var heartPosition = amount - 1;
            
            if (heartPosition < 0)
                heartPosition = 0;
            
            var currentHeartToAnimate = _heartImages[heartPosition].gameObject;
            
            AnimateHeart(currentHeartToAnimate);

            _currentHeart = currentHeartToAnimate;
        }

        private void SetFilledHeartsTo(int amount)
        {
            if (amount < 0)
            {
                Debug.LogError(ConsoleMessages.LifeLowerThanZeroUI);
                amount = 0;
            }
            
            for (int i = 0; i < amount; i++)
            {
                _heartImages[i].sprite = filledHeartSprite;
            }

            for (int i = amount; i < _heartImages.Count; i++)
            {
                _heartImages[i].sprite = emptyHeartSprite;
            }

            var heartPosition = amount - 1;
            
            if (heartPosition < 0)
                heartPosition = 0;
            
            var currentHeartToAnimate = _heartImages[heartPosition].gameObject;
            
            AnimateHeart(currentHeartToAnimate);

            _currentHeart = currentHeartToAnimate;
        }
        
        //TODO: put this methods into SetHeartAmount(int)
        public void AddHeart(int amount)
        {
            SetFilledHeartsTo(amount);
            //TODO custom animations here
        }

        public void RemoveHeart(int amount)
        {
            SetFilledHeartsTo(amount);
            //TODO custom animations here
        }

        /// <summary>
        /// Will cancel _currentHeart animation before to start the new animation.
        /// </summary>
        /// <param name="heart">GameObject to animate.</param>
        private void AnimateHeart(GameObject heart)
        {
            if (_currentHeart != null)
            {
                _currentHeart.transform.localScale = _defaultHeartScale;
                LeanTween.cancel(_currentHeart);
            }

            LeanTween.scale(heart, heartAnimationSize, heartAnimationSpeed)
                .setEaseOutQuad()
                .setEaseInCubic()
                .setLoopPingPong();

        }
    }
}