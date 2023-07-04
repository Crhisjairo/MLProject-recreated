using System;
using _Scripts.Enums;
using _Scripts.SoundsManagers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Characters
{
    [RequireComponent(typeof(SpriteRenderer))]
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
        
        Vector2 _frontDirection = Vector2.down;

        void Awake()
        {
            SetComponents();
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
        
        public void TakeDamage(int damage)
        {
            characterSpecs.TakeDamage(damage);
        }
        
        public void TakeLife(int newLife)
        {
            characterSpecs.TakeLife(newLife);
        }

        public void AddCoins(int amount)
        {
            characterSpecs.AddCoins(amount);
        }
        
        public float GetRunningSpeed()
        {
            return characterSpecs.GetRunningSpeed();
        }

        public float GetSpeed()
        {
            return characterSpecs.GetSpeed();
        }
        
        public void SetCharacterSpecs(CharacterSpecs specs)
        {
            characterSpecs = specs;
        }

        public void PlaySoundSfx(CharacterSfx sound)
        {
            soundEffectsEmitter.Play(sound.ToString(), true);
        }

        public void SetLookingDirection(Vector2 direction)
        {
            //Guardamos la direccion en la que miramos segun el axis
            if (direction.x > 0.01f)
            {
                _frontDirection = Vector2.right;
            }
            if (direction.x < -0.01f)
            {
                _frontDirection = Vector2.left;
            }
            if (direction.y > 0.01f)
            {
                _frontDirection = Vector2.up;
            }
            if (direction.y < -0.01f)
            {
                _frontDirection = Vector2.down;
            }
        }
        
        //TODO maybe move that to SetLookingDirection
        public void SetAnimationByIdleDirection(Vector2 direction)
        {
            float horizontalValue = 0, verticalValue = 0;

            if (_frontDirection.x > 0.01f)
            {
                horizontalValue = 1;
                verticalValue = 0;
            }
            if (_frontDirection.x < -0.01f)
            {
                horizontalValue = -1;
                verticalValue = 0;
            }
            if (_frontDirection.y > 0.01f)
            {
                horizontalValue = 0;
                verticalValue = 1;
            }
            if (_frontDirection.y < -0.01f)
            {
                horizontalValue = 0;
                verticalValue = -1;
            }
            
            _animator.SetFloat(CharacterAnimationValues.LastHorizontal.ToString(), horizontalValue);
            _animator.SetFloat(CharacterAnimationValues.LastVertical.ToString(), verticalValue);
        }

        public void SetAnimationByMovingDirection(Vector2 movement)
        {
            //Animacion
            _animator.SetFloat(CharacterAnimationValues.Horizontal.ToString(), movement.x);
            _animator.SetFloat(CharacterAnimationValues.Vertical.ToString(), movement.y);
            _animator.SetFloat(CharacterAnimationValues.Speed.ToString(), movement.sqrMagnitude); //La velocidad de movimiento
        }
        
        private void SetComponents()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (!TryGetComponent(out soundEffectsEmitter))
            {
                String message = string.Format(ConsoleMessages.OptionalComponentNotFound, typeof(SoundEffectEmitter), name);
                Debug.LogWarning(message);
            }
        }
        
        
    }
}