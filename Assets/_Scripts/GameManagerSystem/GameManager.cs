using System;
using _Scripts.GameManagerSystem.Models;
using _Scripts.Models;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Scripts.GameManagerSystem
{
    
    /// <summary>
    /// Controls players actions like pause game, save data or load data.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private SaveDataWrapper _currentSaveData;

        public static GameManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                var message = string.Format(ConsoleMessages.SingletonError, typeof(GameManager));
                Debug.LogWarning(message);
            }

            DontDestroyOnLoad(gameObject);
            
            LoadGameData();
        }

        public void PauseGameWithKey(InputAction.CallbackContext context)
        {
            if (!context.started)
                return;

            PauseGame();
        }
        
        public void PauseGame()
        {
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void ExitGame()
        {
            Application.Quit();
        }
        
        public bool SaveGameData()
        {
            // TODO: Writes to file
            
            return true;
        }

        private void LoadGameData()
        {
            // TODO: Reads from file
            // JUST FOR TESTING
            SaveDataWrapper data = new SaveDataWrapper(); 

            data.PlayerModel.isAbleToAttack = true;
            data.PlayerModel.isAbleToRun = false;
            data.PlayerModel.isAbleToOpenInventory = false;
            
            _currentSaveData = data;
        }

        public void UpdatePlayerModelSaveData(PlayerModel playerModel)
        {
            _currentSaveData.PlayerModel = playerModel;
        }
        
        
        public void UpdateGameSettingModelSaveData(GameSettingsModel gameSettingsModel)
        {
            _currentSaveData.GameSettingsModel = gameSettingsModel;
        }
        
        public SaveDataWrapper GetSaveData()
        {
            return _currentSaveData;
        }
    }
}