using System;
using _Scripts.Interactuable;
using UnityEngine;
namespace _Scripts.Controllers
{
    
    [RequireComponent(typeof(BoxCollider2D))]
    public class NPCController : MonoBehaviour, IInteractuable
    {
        public SpriteRenderer interactSprite;
        public string inDialogActionMapName = "InDialog";

        Animator _animator;
        bool _playerInRange = false;

        void Awake()
        {
            _animator = GetComponent<Animator>();
            
            interactSprite.gameObject.SetActive(false);
        }

        public void Interact(PlayerController interactor)
        {
            if (!_playerInRange)
                return;
            
            //TODO Interact logic here
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactSprite.gameObject.SetActive(true);
                _playerInRange = true;
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                interactSprite.gameObject.SetActive(false);
                _playerInRange = false;
            }
        }
    }
}
