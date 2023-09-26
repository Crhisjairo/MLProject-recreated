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
    }
}
