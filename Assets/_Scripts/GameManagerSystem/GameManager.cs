using System;
using _Scripts.Enums;
using _Scripts.GameManagerSystem.Models;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Scripts.GameManagerSystem
{
    
    /// <summary>
    /// Controls players actions like pause game, save data or load data.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private string zoneName;

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

        public string GetZoneName()
        {
            return zoneName;
        }

        public void SetZoneName(string newZoneName)
        {
            zoneName = newZoneName;
        }

        public ScenesNames GetCurrentSceneName()
        {
            // Parsing the scene name
            string currentSceneName = SceneManager.GetActiveScene().name;

            Enum.TryParse(currentSceneName, out ScenesNames currentScene);
            
            return currentScene;
        }
        
    }
}