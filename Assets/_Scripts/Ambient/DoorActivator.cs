using _Scripts.Shared.Enums;
using SoundsManagers._Scripts.SoundsManagers;
using UnityEngine;

namespace _Scripts.Ambient
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SoundFXEmitter))]
    public class DoorActivator : TriggerEvent
    {
        private SpriteRenderer _spriteRenderer;
        private SoundFXEmitter _soundFXEmitter;
        
        //TODO: Maybe change this to an Animation.
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        public bool isToggle;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _soundFXEmitter = GetComponent<SoundFXEmitter>();
        }

        [SerializeField] private bool isActive;
        
        public void SetIsActive(bool doorActive)
        {
            this.isActive = doorActive;
            
            _spriteRenderer.sprite = doorActive ? activeSprite : inactiveSprite;

            SoundsFX sfx = doorActive ? SoundsFX.Activated : SoundsFX.Deactivated;
            _soundFXEmitter.PlayOneShot(sfx.ToString());   
        }
        
        public bool GetIsActive()
        {
            return isActive;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(activatorsTag.ToString()))
            {
                if(other.isTrigger && !detectTriggerColliders)
                    return;
                
                SetIsActive(true);
            }
            
            base.OnTriggerEnter2D(other);
        }
    }
}
