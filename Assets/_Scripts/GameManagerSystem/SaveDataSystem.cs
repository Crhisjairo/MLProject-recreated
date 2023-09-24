using System;
using System.IO;
using _Scripts.GameManagerSystem.Models;
using _Scripts.Utils;
using UnityEngine;
namespace _Scripts.GameManagerSystem
{
    public class SaveDataSystem : MonoBehaviour
    {
        public static SaveDataSystem Instance { get; private set; }

        private GameSettingsData _gameSettings;
        
        private PlayerSaveData[] _playerSavesData;

        private const string SaveFileName = "MLProject_SaveData_{0}.sav";
        private const int MaxSavesDataSlots = 3;

        private int _saveDataSlotSelected = 0;
        
        private void Awake()
        {
            CreateSingleton();
            DontDestroyOnLoad(gameObject);
            
            SetComponents();

            LoadSavesData();
        }

        private void SetComponents()
        {
            _gameSettings = new GameSettingsData();
            _playerSavesData = new PlayerSaveData[MaxSavesDataSlots];
            
            for (int i = 0; i < _playerSavesData.Length; i++)
            {
                _playerSavesData[i] = new PlayerSaveData();
            }
        }

        private void CreateSingleton()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                var message = string.Format(ConsoleMessages.SingletonError, typeof(SaveDataSystem));
                Debug.LogWarning(message);
            }
        }
        
        private void LoadSavesData()
        {
            for (int i = 0; i < _playerSavesData.Length; i++)
            {
                string fileName = string.Format(SaveFileName, i);
                string path = Path.Combine(Application.persistentDataPath, fileName);

                string json;
                
                //TODO: parse json to PlayerSaveData
                if(ReadFile(path, out json))
                {
                    var message = $"Successfully loaded {i} data!";
                    
                    Debug.Log(message);
                }
            }
        }
        
        public PlayerSaveData GetPlayerSaveDataSelected()
        {
            return _playerSavesData[_saveDataSlotSelected];
        }
        
        public bool SaveGameData(PlayerSaveData data, int slot)
        {
            string fileName = string.Format(SaveFileName, slot);
            string path = Path.Combine(Application.persistentDataPath, fileName);

            // TODO: serialization here Data to JSON
           var json = "{}";

           if (WriteFile(path, json))
           {
               Debug.Log("Successfully saved data!");
           }


           return true;
        }

        public void SetSaveDataSlot(int slotSelected)
        {
            _saveDataSlotSelected = slotSelected;
        }
        
        public void UpdatePlayerModelSaveData(PlayerSaveData playerSaveData, int slot)
        {
            _playerSavesData[slot] = playerSaveData;
        }
        
        public void UpdateGameSettingModelSaveData(GameSettingsData gameSettingsData)
        {
            _gameSettings = gameSettingsData;
        }

        private bool ReadFile(string path, out string json)
        {
            json = String.Empty;

            try
            {
                json = File.ReadAllText(path);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException)
                {
                    Debug.LogWarning("Save data file do not exist: " + e);
                }
                else if (e is IOException or UnauthorizedAccessException)
                {
                    Debug.LogError("Save data file cannot be read: " + e);
                }
                else
                {
                    Debug.LogError("Unknown error when loading: " + e);
                }
                
                return false;
            }

            return true;
        }

        private bool WriteFile(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("File save data cannot be written: " + e);
                
                return false;
            }
        }
    }
}
