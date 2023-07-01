using System;
using _Scripts.SoundsManagers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Characters
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    
    [System.Serializable]
    public abstract class Character: MonoBehaviour
    {
        public Sprite iconSprite;
        public Sprite characterSprite;
        
        [SerializeField] private SoundEffectEmitter soundEffectsEmitter;
        [SerializeField] private CharacterSpecs characterSpecs;
        [SerializeField] private new ParticleSystem particleSystem;

        Animator _animator;
        SpriteRenderer _spriteRenderer;
        Rigidbody2D _rb;

        void Awake()
        {
            GetComponents();
        }

        public void ExtendsMaxLife()
        {
            characterSpecs.ExtendsMaxLife();
            characterSpecs.SetFullLife();
            //TODO notify HUD
        }

        /**
         * Returns true if player dead.
         */
        public bool TryTakeDamage(int damage)
        {
            characterSpecs.TakeDamage(damage);

            return characterSpecs.HasNoLife();
        }
        
        public bool AddSlotToInventory()
        {
            //TODO Check if can add an slot to inventory.
            
            
            return true;
        }

        public void SetCharacterSpecs(CharacterSpecs specs)
        {
            characterSpecs = specs;
        }
        
        private void GetComponents()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rb = GetComponent<Rigidbody2D>();

            if (!TryGetComponent(out soundEffectsEmitter))
            {
                String message = string.Format(ConsoleMessages.OptionalComponentNotFound, typeof(SoundEffectEmitter), name);
                Debug.LogWarning(message);
            }
        }
    }
}