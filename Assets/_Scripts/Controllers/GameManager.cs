using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    
    /// <summary>
    /// Controls players actions like pause game, save data or load data.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
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
    }
}