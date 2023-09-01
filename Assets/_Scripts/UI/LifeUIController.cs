using System;
using System.Collections.Generic;
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

        public void SetFilledHearts(int amount)
        {
            if (amount < 0)
            {
                Debug.LogError("Life amount cannot be lower than 0.");
                amount = 0;
            }

            for (int i = 0; i < amount; i++)
            {
                Debug.Log("icitte");
                _heartImages[i].sprite = filledHeartSprite;
            }

            for (int i = amount; i < _heartImages.Count; i++)
            {
                Debug.Log("En el segundo");
                _heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}
