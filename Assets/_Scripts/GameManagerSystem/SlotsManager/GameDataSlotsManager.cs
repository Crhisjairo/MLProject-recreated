using _Scripts.GameManagerSystem.Models;
using UnityEngine;

namespace _Scripts.GameManagerSystem.SlotsManager
{
    public class GameDataSlotsManager : MonoBehaviour
    {
        private SaveDataSystem _saveDataSystem;
        
        public Slot[] slots;

        public bool disableEmptySlots = true;
        
        private void Start()
        {
            _saveDataSystem = SaveDataSystem.Instance;

            SetSlotsData();
        }

        private void SetSlotsData()
        {
            PlayerSaveData[] saveDatas = _saveDataSystem.GetPlayerSaveDatas();
            
            for (int i = 0; i < slots.Length; i++)
            {
                var currentSaveData = saveDatas[i];
                
                if (currentSaveData.isNew && disableEmptySlots)
                {
                    slots[i].SetActive(false);
                }
                else if(!currentSaveData.isNew)
                {
                    slots[i].SetSlotName(currentSaveData.slotName);
                    slots[i].SetZoneName(currentSaveData.zoneName);
                    slots[i].SetCoinsAmount(currentSaveData.coinsAmount);

                    slots[i].SetUnlockedCharacters(currentSaveData.characterSaveData);
                }
            }
        }
    }
}
