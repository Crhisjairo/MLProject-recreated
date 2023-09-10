using System;
using System.Collections;
using _Scripts.Enums;
using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.Ambient
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [SerializeField] private Collider2D[] colliders;
        
        private bool isOpen = false;

        [SerializeField] private Animator animator;
        [SerializeField] private TransitionsAnimations onOpenAnimTriggerName, nameOnCloseAnimTrigger;
        [SerializeField] private float waitTimeBeforeStartAnimation = 1f, waitTimeBeforeDisableAnimator = 1f;

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
            
            this.isOpen = isOpen;
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