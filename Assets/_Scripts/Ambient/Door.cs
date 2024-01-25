using System;
using System.Collections;
using UnityEngine;
using System.Linq;
using _Scripts.Shared.Enums;
using SoundsManagers._Scripts.SoundsManagers;
using UnityEngine.Events;

namespace _Scripts.Ambient
{
    public class Door : MonoBehaviour
    {
        private const TransitionsAnimations OnOpenAnimTriggerName = TransitionsAnimations.Open;
        private const TransitionsAnimations OnCloseAnimTriggerName = TransitionsAnimations.Close;

        public UnityEvent onDoorOpen, onDoorClose;
        
        [SerializeField] private SoundFXEmitter soundFXEmitter;
        
        [SerializeField] private Collider2D[] colliders;
        
        private bool _isOpen;

        [SerializeField] private Animator animator;
        
        [SerializeField] protected float waitTimeBeforeStartAnimation = 1f, waitTimeBeforeDisableAnimator = 1f;

        [SerializeField] private DoorActivator[] doorActivators;

        private void Start()
        {
            foreach (var activator in doorActivators)
            {
                activator.onEntityCollides.AddListener(TryToOpen);
            }
            
            if(animator != null)
                animator.enabled = false;
        }

        public void TryToOpen()
        {
            if (doorActivators.All(activator => activator.GetIsActive())
                || doorActivators.Length == 0)
            {
                _isOpen = true;
                onDoorOpen?.Invoke();
                
                PlayOpenAnimation();
            }
        }
        
        public virtual void Close()
        {
            throw new NotImplementedException();
        }
        
        // TODO: must implement an animation to open the door.
        protected virtual void PlayOpenAnimation()
        {
            StartCoroutine(ActiveAnimator(_isOpen));
            
            foreach (var objCollider in colliders)
            {
                objCollider.enabled = !_isOpen;
            }
            
            //soundFXEmitter.PlayOneShot(SoundsFX.Open.ToString());
        }

        private IEnumerator ActiveAnimator(bool isOpen)
        {
            yield return new WaitForSecondsRealtime(waitTimeBeforeStartAnimation);
            
            animator.enabled = true;
            
            if(isOpen)
                animator.SetTrigger(OnOpenAnimTriggerName.ToString());
            else
                animator.SetTrigger(OnCloseAnimTriggerName.ToString());
            
            yield return new WaitForSecondsRealtime(waitTimeBeforeDisableAnimator);

            animator.enabled = false;
        }

        private void OnDisable()
        {
            foreach (var activator in doorActivators)
            {
                activator.onEntityCollides.RemoveListener(TryToOpen);
            }
        }
    }
}
