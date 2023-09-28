using System;
using System.IO;
using _Scripts.Controllers;
using _Scripts.GameManagerSystem.Models;
using _Scripts.Utils;
using UnityEngine;
namespace _Scripts.GameManagerSystem
{
    public class SaveDataSystem : MonoBehaviour
    {
        private GameSettingsData _gameSettings;
        
        private PlayerSaveData[] _playerSavesData;

        private const string SaveFileName = "MLProject_SaveData_{0}.sav";
        private const int MaxSavesDataSlots = 3;

        private int _saveDataSlotSelected = 0;
        private string _selectedSlotName = string.Empty;
        
        private const int MaxSlotNameSize = 14;
        
        public static SaveDataSystem Instance;
        
        private void Awake()
        {
            DontDestroyOnLoad (this);
		    
            CreateSingleton();
        }

        private void CreateSingleton()
        {
            if (Instance == null) {
                Instance = this;
                
                SetComponents();
                LoadSavesData();
            } else {
                Destroy(gameObject);
            }
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
                    _playerSavesData[i] = JsonUtility.FromJson<PlayerSaveData>(json);
                  
                    //When JSON is empty, it considers like it is a new gamefile
                    
                    var message = $"Successfully loaded {i} data!";
                    Debug.Log(message);
                    
                }
            }
        }

        public PlayerSaveData GetPlayerSaveDataSelected()
        {
            return _playerSavesData[_saveDataSlotSelected];
        }

        public void CreateNewGameSaveFile()
        {
            var newSaveData = new PlayerSaveData();

            newSaveData.isNew = false;
            newSaveData.slotName = _selectedSlotName;

            Debug.Log($"Creating new game at {_saveDataSlotSelected} with name {newSaveData.slotName}!");
            
            _playerSavesData[_saveDataSlotSelected] = newSaveData;
        }

        public void SetSelectedSlotName(string slotName)
        {
            if (slotName.Length > MaxSlotNameSize)
            {
                slotName = slotName.Substring(0, MaxSlotNameSize);
            }
            
            _selectedSlotName = slotName;
        }

        public bool SaveGameData(PlayerSaveData newData)
        {
            string fileName = string.Format(SaveFileName, _saveDataSlotSelected);
            string path = Path.Combine(Application.persistentDataPath, fileName);

            _playerSavesData[_saveDataSlotSelected] = newData;
            var dataToSave = _playerSavesData[_saveDataSlotSelected];
            
            // TODO: serialization here Data to JSON
           var json = JsonUtility.ToJson(dataToSave);

           if (WriteFile(path, json))
           {
               Debug.Log("Successfully saved data!\n" + json);
           }

           return true;
        }

        public void SetSaveDataSlot(int slotSelected)
        {
            _saveDataSlotSelected = slotSelected;
        }

        

        public PlayerSaveData[] GetPlayerSaveDatas()
        {
            return _playerSavesData;
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

            if (json == string.Empty)
            {
                Debug.LogWarning("Save data file is empty or corrupted!" + path);
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
