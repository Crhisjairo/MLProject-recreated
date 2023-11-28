using System;
using _Scripts.Controllers;
using _Scripts.DialogSystem;
using _Scripts.Interfaces;
using UnityEngine;
namespace _Scripts.Ambient
{
    [RequireComponent(typeof(DialogTrigger))]
    public class Sign : MonoBehaviour, IInteractable
    {
        private DialogTrigger _dialogTrigger;
        
        private void Awake()
        {
            _dialogTrigger = GetComponent<DialogTrigger>();
        }

        public void Interact(PlayerController interactor)
        {
            _dialogTrigger.TryToStartDialogs(interactor);
        }
    }
}
