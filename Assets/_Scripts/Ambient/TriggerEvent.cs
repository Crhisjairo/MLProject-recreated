using _Scripts.Shared.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Ambient
{ 
    public class TriggerEvent : MonoBehaviour
    {
        [SerializeField] private Collider2D[] collidersToDisable;
        [SerializeField] protected bool detectTriggerColliders;
        
        public Tags activatorsTag;
        
        public UnityEvent onEntityCollides;
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(activatorsTag.ToString()))
            {
                if(other.isTrigger && !detectTriggerColliders)
                    return;
                
                onEntityCollides?.Invoke();

                foreach (var colliderToDisable in collidersToDisable)
                {
                    colliderToDisable.enabled = false;
                }
                
            }
        }

    }
}
