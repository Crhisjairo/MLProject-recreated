using System;
using _Scripts.Controllers;
using _Scripts.Controllers.Interfaces;
using _Scripts.DialogSystem;
using UnityEngine;
namespace _Scripts.Ambient
{
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
