using System;
using System.Linq;
using _Scripts.Enums;
using _Scripts.GameManagerSystem.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Scripts.GameManagerSystem.SlotsManager
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Button slotButton; 
        
        [SerializeField] private TextMeshProUGUI slotName;
        [SerializeField] private TextMeshProUGUI zoneName;
        [SerializeField] private TextMeshProUGUI coinsAmount;

        [SerializeField] private GameObject[] charactersToUnlockGo;

        public void SetSlotName(string slotName)
        {
            this.slotName.text = slotName;
        }

        public void SetZoneName(string zoneName)
        {
            this.zoneName.text = zoneName;
        }
        
        public void SetCoinsAmount(int amount)
        {
            coinsAmount.text = amount.ToString();
        }

        public void SetActive(bool isActive)
        {
            slotButton.interactable = isActive;
        }

        public void SetUnlockedCharacters(CharacterSaveData[] saveDatas)
        {
            CharacterNames[] charactersNames = saveDatas
                .Select(data => data.characterName)
                .ToArray();

            foreach (var character in charactersToUnlockGo)
            {
                Enum.TryParse(character.name, out CharacterNames goName);

                var currentData = saveDatas.FirstOrDefault(x=> x.characterName == goName);
                
                if (charactersNames.Contains(goName))
                {
                    //TODO: optimise xddd
                    var lifeText = character
                        .GetComponentInChildren<Transform>()
                        .GetChild(0)
                        .GetChild(1).gameObject
                        .GetComponent<TextMeshProUGUI>();
                    
                    character.SetActive(true);
                    lifeText.text = currentData?.currentLife.ToString();
                }
            }
        }
        
        public string GetSlotName()
        {
            return slotName.text;
        }
    }
}
