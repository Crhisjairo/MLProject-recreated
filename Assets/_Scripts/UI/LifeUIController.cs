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

        private List<Image> _heartImages;
        
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
            
            //TODO Set custom animation here
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
        }

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
    }
}
