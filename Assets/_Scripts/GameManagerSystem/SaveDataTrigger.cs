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

        public void SaveGame()
        {
            //TODO: show save game icon
            var data = playerController.BuildPlayerSaveData();

            data.lastSceneName = gameManager.GetCurrentSceneName();
            data.zoneName = gameManager.GetZoneName();

            _saveDataSystem.SaveGameData(data);
            
            Debug.Log("Saving from SaveGameTrigger!");
        }
    }
}
