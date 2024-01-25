using _Scripts.Shared.Enums;
using TMPro;
using UnityEngine;

namespace _Scripts.GameManagerSystem.SlotsManager
{
    public class CharacterSlotWrapper : MonoBehaviour
    {
        public CharacterNames CharacterName;
        
        [SerializeField] private TextMeshProUGUI lifeAmountText;
        [SerializeField] private bool isActive;

        private void Awake()
        {
            if(isActive)
                Show(0);
            else
                Hide();
        }

        public void Show(int lifeAmount)
        {
            isActive = true;
            gameObject.SetActive(isActive);
            
            lifeAmountText.text = lifeAmount.ToString();
        }

        public void Hide()
        {
            isActive = false;
            gameObject.SetActive(isActive);
            
        }
    }
}