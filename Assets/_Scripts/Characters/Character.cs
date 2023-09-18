using System;
using _Scripts.Enums;
using _Scripts.SoundsManagers;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Characters
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    
    [System.Serializable]
    public abstract class Character: MonoBehaviour
    {
        public static int CurrentCoins { private set; get; }
        
        public Sprite iconSprite;
        public Sprite characterSprite;

        [FormerlySerializedAs("soundEmitter")]
        [SerializeField] SoundFXEmitter soundFXEmitter;
        [SerializeField] new ParticleSystem particleSystem;

        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private CapsuleCollider2D _capsuleCollider;

        private Vector2 _lookingDirection = Vector2.down;
        
        #region Character specs

        public string CharacterName { protected set; get; }
        [SerializeField] int maxLife = 13;
        [SerializeField] int currentLife = 3;
        [SerializeField] float speed = 2f;
        [SerializeField] float runningSpeed = 3f;
        [SerializeField] int attackDamage = 1;
        [SerializeField] float forceImpulse = 500;

        #endregion

        protected virtual void Awake()
        {
            SetComponents();
        }

        public void AddExtraHeartSlot()
        {
            maxLife++;
            
            SetFullLife();
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

        public int GetAttackDamage()
        {
            return attackDamage;
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
            if (currentLife <= 0)
            {
                return;
            }
            
            currentLife -= damage;
        }

        public bool HasNoLife() => currentLife <= 0;
        
        public void TakeLife(int amount)
        {
            currentLife += amount;
            
            if (currentLife >= maxLife)
            {
                currentLife = maxLife;
            }
        }

        public void AddToMaxLife(int amount)
        {
            maxLife += amount;
        }
        
        public int GetCurrenLife()
        {
            return currentLife;
        }

        public int GetMaxLife()
        {
            return maxLife;
        }

        public float GetForceImpulse()
        {
            return forceImpulse;
        }
        
        private void SetFullLife()
        {
            currentLife = maxLife;
        }

        public void PlaySoundSfx(SoundsFX sound)
        {
            soundFXEmitter.PlayOneShot(sound.ToString());
        }

        public void ResetLastLookingDirection()
        {
            _lookingDirection = new Vector2();
            
            _animator.SetFloat(CharacterAnimationParameters.LastHorizontal.ToString(), 0);
            _animator.SetFloat(CharacterAnimationParameters.LastVertical.ToString(), 0);
        }
        
        public void SetLookingDirection(Vector2 direction)
        {
            //Guardamos la direccion en la que miramos segun el axis
            if (direction.x > 0.01f)
            {
                _lookingDirection = Vector2.right;
            }
            if (direction.x < -0.01f)
            {
                _lookingDirection = Vector2.left;
            }
            if (direction.y > 0.01f)
            {
                _lookingDirection = Vector2.up;
            }
            if (direction.y < -0.01f)
            {
                _lookingDirection = Vector2.down;
            }
        }

        public Vector2 GetLookingDirection()
        {
            return _lookingDirection;
        }
        
        //TODO maybe move that to SetLookingDirection
        public void SetAnimationByIdleDirection(Vector2 direction)
        {
            float horizontalValue = 0, verticalValue = 0;

            if (_lookingDirection.x > 0.01f)
            {
                horizontalValue = 1;
                verticalValue = 0;
            }
            if (_lookingDirection.x < -0.01f)
            {
                horizontalValue = -1;
                verticalValue = 0;
            }
            if (_lookingDirection.y > 0.01f)
            {
                horizontalValue = 0;
                verticalValue = 1;
            }
            if (_lookingDirection.y < -0.01f)
            {
                horizontalValue = 0;
                verticalValue = -1;
            }
            
            _animator.SetFloat(CharacterAnimationParameters.LastHorizontal.ToString(), horizontalValue);
            _animator.SetFloat(CharacterAnimationParameters.LastVertical.ToString(), verticalValue);
        }

        public void SetSpeedAnimationValueByMovement(Vector2 movement)
        {
            _animator.SetFloat(CharacterAnimationParameters.Speed.ToString(), movement.sqrMagnitude);
        }

        public ParticleSystem GetParticleSystem()
        {
            return particleSystem;
        }

        public void SetColliderActive(bool isActive)
        {
            _capsuleCollider.enabled = isActive;
        }
        
        private void SetComponents()
        {
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();

            if (!TryGetComponent(out soundFXEmitter))
            {
                String message = string.Format(ConsoleMessages.OptionalComponentNotFound, typeof(SoundFXEmitter), name);
                Debug.LogWarning(message);
            }
        }
        
        
    }
}