using System;
using _Scripts.GameManagerSystem.Models;
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
        public static GameManager Instance { get; private set; }

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
        
        
    }
}