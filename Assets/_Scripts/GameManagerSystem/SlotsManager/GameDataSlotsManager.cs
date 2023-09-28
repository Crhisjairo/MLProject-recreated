using System.Collections.Generic;
using System.Linq;
using _Scripts.Enums;
using _Scripts.GameManagerSystem.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.GameManagerSystem.SlotsManager
{
    public class GameDataSlotsManager : MonoBehaviour
    {
        private SaveDataSystem saveDataSystem;
        
        public Slot[] slots;

        public bool disableEmptySlots = true;
        
        private void Start()
        {
            saveDataSystem = SaveDataSystem.Instance;

            SetSlotsData();
        }

        private void SetSlotsData()
        {
            PlayerSaveData[] saveDatas = saveDataSystem.GetPlayerSaveDatas();
            
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

                    slots[i].SetUnlockedCharacters(currentSaveData.CharacterSaveData);
                }
            }
        }
    }
}
