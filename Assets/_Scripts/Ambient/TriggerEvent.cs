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
        [SerializeField] private EdgeCollider2D edgeColliderToDisable;
        [SerializeField] private BoxCollider2D boxColliderToDisable;

        public Tags tagToCheckCollisions;
        
        public UnityEvent onEntityCollides;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(tagToCheckCollisions.ToString()))
            {
                onEntityCollides?.Invoke();
                
                if(edgeColliderToDisable)
                    edgeColliderToDisable.enabled = false;
                if(boxColliderToDisable)
                    boxColliderToDisable.enabled = false;
            }
        }
    }
}
