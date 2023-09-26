using _Scripts.GameManagerSystem.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.GameManagerSystem.SlotsManager
{
    public class GameDataSlotsManager : MonoBehaviour
    {
        public Slot[] slots;

        public bool disableEmptySlots = true;
        
        private void Start()
        {
            SetSlotsData();
        }

        private void SetSlotsData()
        {
            PlayerSaveData[] saveDatas = SaveDataSystem.Instance.GetPlayerSaveDatas();
            
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
                }
            }
        }
    }
}
