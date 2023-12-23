using UnityEngine;

namespace _Scripts.Ambient
{
    public class DoorActivator : TriggerEvent
    {
        [SerializeField] private bool isActive;
        
        public virtual void SetIsActive(bool isActive)
        {
            this.isActive = isActive;
        }
        
        public bool GetIsActive()
        {
            return isActive;
        }
    }
}
