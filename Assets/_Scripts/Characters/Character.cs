using System;
using _Scripts.Enums;
using _Scripts.SoundsManagers;
using _Scripts.Utils;
using UnityEngine;

namespace _Scripts.Characters
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    
    [System.Serializable]
    public abstract class Character: MonoBehaviour
    {
        public Sprite iconSprite;
        public Sprite characterSprite;
        
        [SerializeField] SoundEffectEmitter soundEffectsEmitter;
        [SerializeField] new ParticleSystem particleSystem;
        Animator _animator;
        SpriteRenderer _spriteRenderer;
        
        Vector2 _frontDirection = Vector2.down;
        
        #region Character specs
        
        public static int CurrentCoins { private set; get; }

        public string CharacterName { protected set; get; }
        [SerializeField] int maxLife = 13;
        [SerializeField] int currentLife = 3;
        [SerializeField] float speed = 2f;
        [SerializeField] float runningSpeed = 3f;
        [SerializeField] int attackDamage = 1;

        #endregion

        protected virtual void Awake()
        {
            SetComponents();
        }

        public void ExtendsMaxLife()
        {
            maxLife++;
            SetFullLife();
            //TODO notify HUD
        }
        
        public void AddCoins(int amount)
        {
            CurrentCoins += amount;
        }

        /**
         * Returns true if player is dead.
         */
        public bool TryTakeDamage(int damage)
        {
            TakeDamage(damage);

            return HasNoLife();
        }
        
        public float GetRunningSpeed()
        {
            return runningSpeed;
        }

        public float GetSpeed()
        {
            return speed;
        }
        
        public bool AddSlotToInventory()
        {
            //TODO Check if can add an slot to inventory.
            
            
            return true;
        }
        
        public void TakeDamage(int damage)
        {
            currentLife -= damage;
        }

        public bool HasNoLife() => currentLife <= 0;
        
        public void TakeLife(int newLife)
        {
            if (currentLife >= maxLife)
            {
                return;
            }
        
            currentLife += newLife;
        }

        private void SetFullLife()
        {
            currentLife = maxLife;
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