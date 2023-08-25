using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    
    /// <summary>
    /// Controls players actions like open inventory, pause game, save data or load data.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class GameManager : MonoBehaviour
    {
        PlayerInput _playerInput;
        InputActionMap _lastActionMap, _currentActionMap;

        void Awake()
        {
            SetComponents();
        }

        public void PauseGameWithKey(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            Time.timeScale = 0;
        }

        public void ResumeGame()
        {
            Time.timeScale = 1;
        }

        public void ChangeActionMapTo(string inputMap)
        {
            _playerInput.SwitchCurrentActionMap(inputMap);
        }
        
        void SetComponents()
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        
    }
}