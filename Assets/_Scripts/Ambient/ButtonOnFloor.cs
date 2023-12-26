using System;
using _Scripts.SoundsManagers;
using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.Ambient
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ButtonOnFloor : DoorActivator
    {
        private SpriteRenderer _spriteRenderer;
        
        //TODO: Maybe change this to an Animation.
        [SerializeField] private Sprite activeSprite;
        [SerializeField] private Sprite inactiveSprite;
        
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void SetIsActive(bool isActive)
        {
            base.SetIsActive(isActive);
            
            _spriteRenderer.sprite = isActive ? activeSprite : inactiveSprite;
        }
        
    }
}
