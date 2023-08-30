using System;
using _Scripts.DialogSystem;
using _Scripts.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
namespace _Scripts.Controllers
{
    
    [RequireComponent(typeof(BoxCollider2D))]
    public class NpcInteraction : MonoBehaviour, IInteractuable
    {
        public SpriteRenderer interactSprite;
        public string inDialogActionMapName = "InDialog";

        public bool lookAtPlayerWhenInteracting;
        
        private Animator _animator;
        private DialogTrigger _dialogueTrigger;
        private bool _playerInRange = false;

        [SerializeField] private UnityEvent onAbleToInteract; 
        [SerializeField] private UnityEvent onUnableToInteract;    
        
        void Awake()
        {
            _animator = GetComponent<Animator>();
            _dialogueTrigger = GetComponent<DialogTrigger>();

            interactSprite.gameObject.SetActive(false);
        }

        public void Interact(PlayerController interactor)
        {
            if (!_playerInRange)
                return;
            
            if (_dialogueTrigger is not null)
            {
                _dialogueTrigger.SetDialogByContext(interactor);
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactSprite.gameObject.SetActive(true);
                _playerInRange = true;
                
                onAbleToInteract?.Invoke();
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactSprite.gameObject.SetActive(false);
                _playerInRange = false;
                
                onUnableToInteract?.Invoke();
            }
        }
    }
}
