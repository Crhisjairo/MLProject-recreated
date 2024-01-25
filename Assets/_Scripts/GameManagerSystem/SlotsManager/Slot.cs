using System.Linq;
using _Scripts.GameManagerSystem.Models;
using _Scripts.Shared.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.GameManagerSystem.SlotsManager
{
    public class Slot : MonoBehaviour
    {
        [SerializeField] private Button slotButton; 
        
        [SerializeField] private TextMeshProUGUI slotName;
        [SerializeField] private TextMeshProUGUI zoneName;
        [SerializeField] private TextMeshProUGUI coinsAmount;

        [SerializeField] private CharacterSlotWrapper[] characterWrappers;
        
        public void SetSlotName(string newSlotName)
        {
            slotName.text = newSlotName;
        }

        public void SetZoneName(string newZoneName)
        {
            zoneName.text = newZoneName;
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
            saveDatas.ToList().ForEach(data =>
            {
                CharacterNames charName = data.characterName;
                
                characterWrappers.ToList().ForEach(wrapper =>
                {
                    if (wrapper.CharacterName.Equals(charName))
                        wrapper.Show(data.currentLife);
                });
            });
        }
        
        public string GetSlotName()
        {
            return slotName.text;
        }
    }
}
