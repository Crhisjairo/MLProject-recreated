using _Scripts.Shared.Enums;
using _Scripts.SoundsManagers;
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
        public bool isToggle = false;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _soundFXEmitter = GetComponent<SoundFXEmitter>();
        }

        [SerializeField] private bool isActive;
        
        public void SetIsActive(bool isActive)
        {
            this.isActive = isActive;
            
            _spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;

            SoundsFX sfx = isActive ? SoundsFX.Activated : SoundsFX.Deactivated;
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
                
                Debug.Log(other.gameObject.name);
                
                SetIsActive(true);
            }
            
            base.OnTriggerEnter2D(other);
        }
    }
}
