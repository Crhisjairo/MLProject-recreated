using _Scripts.Ambient;
using UnityEngine;
using UnityEngine.EventSystems;
namespace _Scripts.Interfaces
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
