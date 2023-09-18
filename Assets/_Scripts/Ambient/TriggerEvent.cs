using System;
using System.Linq;
using _Scripts.DialogSystem;
using _Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace _Scripts.Ambient
{ 
    public class TriggerEvent : MonoBehaviour
    {
        [SerializeField] private Collider2D[] collidersToDisable;
        [SerializeField] private bool enableTriggerColliders = false;
        
        
        public Tags tagToCheckCollisions;
        
        public UnityEvent onEntityCollides;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(tagToCheckCollisions.ToString()))
            {
                if(other.isTrigger && !enableTriggerColliders)
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
