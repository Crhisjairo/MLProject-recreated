using System;
using _Scripts.Controllers;
using _Scripts.GameManagerSystem.Models;
using UnityEngine;
namespace _Scripts.GameManagerSystem
{
    public class SaveDataTrigger : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        [SerializeField] private GameManager gameManager;

        private SaveDataSystem _saveDataSystem;
        
        private void Start()
        {
            _saveDataSystem = SaveDataSystem.Instance;
        }

        public void SaveGame(bool isAutoSave)
        {
            // Last player position must just be included when an auto save is not triggered.
            // Only includes last player position when is a manual save.
            bool includePlayerPosition = !isAutoSave; 
                
            //TODO: show save game icon
            var data = playerController.BuildPlayerSaveData(includePlayerPosition);

            data.isAutoSaved = isAutoSave;

            data.lastSceneName = gameManager.GetCurrentSceneName();
            data.zoneName = gameManager.GetZoneName();

            _saveDataSystem.SaveGameData(data);
            
            Debug.Log("Saving from SaveGameTrigger!");
        }
    }
}
