using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Enums;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;
using _Scripts.Controllers.Enemies.Interfaces;
using _Scripts.SoundsManagers;

namespace _Scripts.Ambient
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SoundFXEmitter soundFXEmitter;
        
        [SerializeField] private Collider2D[] colliders;

        
        private bool isOpen = false;

        [SerializeField] private Animator animator;
        [SerializeField] private TransitionsAnimations onOpenAnimTriggerName, nameOnCloseAnimTrigger;
        [SerializeField] private float waitTimeBeforeStartAnimation = 1f, waitTimeBeforeDisableAnimator = 1f;

        [SerializeField] private DoorActivator[] doorActivators;

        private void Start()
        {
            animator.enabled = false;
        }

        public void IsOpen(bool isOpen)
        {
            StartCoroutine(ActiveAnimator(isOpen));
            
            foreach (var collider in colliders)
            {
                collider.enabled = !isOpen;
            }
            
            soundFXEmitter.PlayOneShot("open-door");
            
            this.isOpen = isOpen;
        }

        public void TryToOpen()
        {
            if (doorActivators.All(activator => activator.GetIsActive()))
            {
                IsOpen(true);
            }
        }

        private IEnumerator ActiveAnimator(bool isOpen)
        {
            yield return new WaitForSecondsRealtime(waitTimeBeforeStartAnimation);
            
            animator.enabled = true;
            
            if(isOpen)
                animator.SetTrigger(onOpenAnimTriggerName.ToString());
            else
                animator.SetTrigger(nameOnCloseAnimTrigger.ToString());
            
            yield return new WaitForSecondsRealtime(waitTimeBeforeDisableAnimator);

            animator.enabled = false;
        }
    }
}
