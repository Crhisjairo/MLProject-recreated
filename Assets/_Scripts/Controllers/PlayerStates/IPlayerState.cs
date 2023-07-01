using UnityEngine.InputSystem;
namespace _Scripts.Controllers.PlayerStates
{
    public interface IPlayerState
    {
        public void HandleDpadInput(InputAction.CallbackContext inputContext);
        
        public void HandleAttackInput(InputAction.CallbackContext inputContext);
        
        public void HandleRunInput(InputAction.CallbackContext inputContext);
        
        public void HandleInventoryInput(InputAction.CallbackContext inputContext);
        
        public void HandleInteractInput(InputAction.CallbackContext inputContext);

        public void HandleChangeCharacterInput(InputAction.CallbackContext inputContext);

        public void HandlePauseInput(InputAction.CallbackContext inputContext);
    }
}
