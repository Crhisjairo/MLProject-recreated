using System;
using _Scripts.Controllers;
using UnityEngine;
namespace _Scripts.GameManagerSystem
{
    public class SaveGameTrigger : MonoBehaviour
    {
        private SaveDataSystem _saveDataSystem;

        [SerializeField] private PlayerController playerController;
        
        private void Awake()
        {
            _saveDataSystem = SaveDataSystem.Instance;
        }

        public void SaveGame()
        {
            //TODO: show save game icon
            Debug.Log("Saving from SaveGameTrigger!");
        }
    }
}
