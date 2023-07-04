using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    public class InputPlayerHandler : MonoBehaviour
    {
        private PlayerInput PlayerInput;
        private InputActionMap _lastActionMap, _currentActionMap;

        private void Start()
        {
            //currentState = new InGame(this);
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            TogglePauseGame();

            //TODO Change InputMap and notify HUD
            
        }
        
        
        public void TogglePauseGame()
        {
            //TODO Move this to GameManager
            /**
            GameInPause = !GameInPause;
    
            Time.timeScale = GameInPause ? 0 : 1;
            **/
        }
        
        void SetComponents()
        {
            PlayerInput = GetComponent<PlayerInput>();
        }
        
    }
}