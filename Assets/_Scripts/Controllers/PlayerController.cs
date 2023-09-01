using System.Collections;
using _Scripts.Characters;
using _Scripts.Enums;
using _Scripts.Interfaces;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask _enemyLayers;
        [SerializeField] GameObject[] _charactersModels;
        [SerializeField] int startingCharacterIndex = 0;

        [SerializeField] float _defaultInvulnerabilityTime = 0.7f;

        public float attackRate = 2f; // TODO maybe move that to Character class
        public float attackRange = 1f; // TODO maybe move that to Character class
        public Vector2 distanceAttackRange; // TODO maybe move that to Character class
        public float _nextAttackTime; // TODO maybe move that to Character class
        public float interactionDistance = 0.5f;

        /// <summary>
        /// It pass the current amount of the maximum life.
        ///
        /// Ex: maxLife = 12
        /// </summary>
        public UnityEvent<int> onMaxLifeUpdate;

        /// <summary>
        /// It pass the current amount of life when life getted.
        ///
        /// Ex: life = 5
        /// </summary>
        public UnityEvent<int> onLifeGetted;
        
        /// <summary>
        /// It pass the current amount of life when damage taken.
        ///
        /// Ex: life = 4
        /// </summary>
        public UnityEvent<int> onDamageTaken;

        /// <summary>
        /// It pass the current amount of coins.
        ///
        /// Ex: coins = 10
        /// </summary>
        public UnityEvent<int> onCoinsUpdate;
        
        /// <summary>
        /// It pass the current active character.
        /// </summary>
        public UnityEvent<Character> onCharacterChange;

        private bool IsInvulnerable { set; get; }

        private CharactersManager _characterManager;
        private PlayerInput _playerInput;

        private Rigidbody2D _rb;

        private float diagonalLimiter = 0.9f; // 0.7 default
        private Vector2 movement;
        
        private float currentSpeed;
        
        void Awake()
        {
            SetComponents();
        }
        
        void Start()
        {
            currentSpeed = _characterManager.ActiveCharacter.GetSpeed();
            
           UpdaterAllPlayerUI();
        }
        
        void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + movement * (currentSpeed * Time.fixedDeltaTime));
        }

        /// <summary>
        /// This method must be called JUST WHEN NECESSARY, because it updates all player UI elements.
        /// </summary>
        public void UpdaterAllPlayerUI()
        {
            onMaxLifeUpdate?.Invoke(_characterManager.ActiveCharacter.GetMaxLife());
            onLifeGetted?.Invoke(_characterManager.ActiveCharacter.GetCurrenLife());
            onCoinsUpdate?.Invoke(Character.CurrentCoins);
            onCharacterChange?.Invoke(_characterManager.ActiveCharacter);
        }
        
        public void Move(InputAction.CallbackContext inputContext)
        {
            var inputMovement = inputContext.ReadValue<Vector2>();
            
            movement.x = 0; 
            movement.y = 0;
                
            // limit movement speed diagonally, so you move at 70% speed
            if (inputMovement.x != 0 && inputMovement.y != 0) 
            {
                inputMovement.x *= diagonalLimiter;
                inputMovement.y *= diagonalLimiter;
            }
                
            movement.x = inputMovement.x;
            movement.y = inputMovement.y; 
            
            var activeCharacter = _characterManager.ActiveCharacter;
            
            activeCharacter.SetLookingDirection(movement);
            activeCharacter.SetAnimationByIdleDirection(movement);
            
            activeCharacter.SetSpeedAnimationValueByMovement(movement);
        }
        
        public void StartRunning(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.performed)
                return;
            
            currentSpeed = _characterManager.ActiveCharacter.GetRunningSpeed();
            _characterManager.ActiveAnimator.SetBool(CharacterAnimationStates.Running.ToString(), true);
        }

        public void StopRunning(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.canceled)
                return;
            
            currentSpeed = _characterManager.ActiveCharacter.GetSpeed();
            _characterManager.ActiveAnimator.SetBool(CharacterAnimationStates.Running.ToString(), false);
        }
        
        public void Attack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (Time.time >= _nextAttackTime)
            {
                //Animamos al personaje
                _characterManager.ActiveAnimator.SetTrigger("Attack");
                
                //Calculamos la direccion del ataque
                //Creamos la direcciôn y la distance de ataque relativa al personaje
                Vector2 attackOffset = CalculateAttackOffset();

                //Ponemos las particulas en la misma posiciôn de ataque
                _characterManager.ActiveParticleSystem.gameObject.transform.position = new Vector3(attackOffset.x, attackOffset.y, -1);
                _characterManager.ActiveParticleSystem.Play();
                
                //Reproducimos el sonido
                _characterManager.ActiveCharacter.PlaySoundSfx(CharacterSfx.UnicornAttackSfx);
                
                //Atacamos a los enemigos
                AttackEnnemiesOnOverlapCircle(attackOffset);
                
                //Recalculamos el tiempo segun el attackRate
                _nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        public void TakeDamage(int damageAmount)
        {
            //Verificamos que no seamos invulnerables
            if (IsInvulnerable)
            {
                return;
            }
            
            ShakeCamera(1.5f, 0.1f);
            
            _characterManager.ActiveAnimator.SetTrigger(CharacterAnimationStates.Damaged.ToString());
            
            ActivateInvulnerability(_defaultInvulnerabilityTime);
            
            _characterManager.ActiveCharacter.TakeDamage(damageAmount);
            
            onDamageTaken?.Invoke(_characterManager.ActiveCharacter.GetCurrenLife());
        }
        
        public void TakeLife(int lifeAmount)
        {
            _characterManager.ActiveCharacter.TakeLife(lifeAmount);
            
            onLifeGetted?.Invoke(_characterManager.ActiveCharacter.GetCurrenLife());
        }

        public void TakeExtraHeart(int amount)
        {
            _characterManager.ActiveCharacter.AddToMaxLife(amount);
            
            onMaxLifeUpdate?.Invoke(_characterManager.ActiveCharacter.GetMaxLife());
        }
        
        public void AddCoins(int amount)
        {
            _characterManager.ActiveCharacter.AddCoins(amount);
            
            onCoinsUpdate?.Invoke(Character.CurrentCoins);
        }

        public void PlayAnimation(string animName)
        {
            //_characterUtil.ActiveAnimator.Play(animName);
            _characterManager.ActiveAnimator.SetTrigger(animName);
        }
        
        public void CheckFrontInteraction(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.performed)
                return;
            
            Vector2 origin = transform.position;
            Vector2 lookingDirection = _characterManager.ActiveCharacter.GetLookingDirection();
            
            // The ~ operator inverts a bitmask, so it detects collide against everything except layer "Player"
            Collider2D collider = Physics2D.OverlapCircle(origin, interactionDistance, ~LayerMask.GetMask("Player"));
            
            if (collider)
            {
                IInteractuable interactuable = collider.gameObject.GetComponent<IInteractuable>();
                interactuable.Interact(this);
            }
        }
        
        #region Calculations

            /// <summary>
        /// Calcula la distancia entre el jugador y la zona de ataque en base a Distance Attack Range.
        /// Si el radio de ataque aumenta, la distancia de ataque tambiên deberîa aumentar
        /// </summary>
        /// <returns>Posicion del centro de ataque relativo al personaje</returns>
        public Vector2 CalculateAttackOffset()
        {
            Vector3 position = transform.position;
            
            Vector2 lookingDirection = Vector2.down;

            // This condition is added to draw gizmos on debug
            if (_characterManager is not null)
            {
                lookingDirection = _characterManager.ActiveCharacter.GetLookingDirection();
            }
            
            Vector2 relDirection =  lookingDirection.normalized + new Vector2(position.x, position.y);

            if (lookingDirection.x > 0.01) //Derecha
            {
                relDirection.x += distanceAttackRange.x;
            }

            if (lookingDirection.x < -0.01) //Izquierda
            {
                relDirection.x -= distanceAttackRange.x;
            }
            
            if (lookingDirection.y > 0.01) //Derecha
            {
                relDirection.y += distanceAttackRange.y;
            }

            if (lookingDirection.y < -0.01) //Izquierda
            {
                relDirection.y -= distanceAttackRange.y;
            }

            return relDirection;
        }
        
        private IEnumerator StartInvulnerabilityTimer(float time)
        {
            Coroutine flashing = StartCoroutine(StartFlashing());
            IsInvulnerable = true;
            
            yield return new WaitForSeconds(time);
            
            StopCoroutine(flashing);
            IsInvulnerable = false;
            
            //Nos aseguramos de quedarnos blancos xd
            _characterManager.ActiveSpriteRenderer.material.color = Color.white;
            
            //GameManager.Instance.CanPlayerInteract = true;
        }
        
        IEnumerator StartFlashing()
        {
            var spriteRenderer = _characterManager.ActiveSpriteRenderer;
            
            while (true)
            {
                spriteRenderer.color = Color.red;
                yield return new WaitForSeconds(0.09f);
                
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.09f);
            }
        }
        
        void ShakeCamera(float intensity, float time)
        {
            // CinemachineBasicMultiChannelPerlin cinema = GameManager.Instance.vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            // cinema.m_AmplitudeGain = intensity;

            StartCoroutine(ShakeCounter(time));
        }

        private IEnumerator ShakeCounter(float time)
        {
            yield return new WaitForSeconds(time);
            // GameManager.Instance.vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
        }
        
        public void AttackEnnemiesOnOverlapCircle(Vector2 attackOffset)
        {
            Collider2D[] hitEnnemies = Physics2D.OverlapCircleAll(attackOffset, attackRange, _enemyLayers);
            
            //Hacemos dano a los enemigos
            foreach (var enemyCollider in hitEnnemies)
            {
                if (!enemyCollider.isTrigger)
                {
                    //Debug.Log("Enemy hit: " + enemyCollider.name);
                    // enemyCollider.GetComponent<Enemy>().OnEnemyAttacked(currentCharacterSpecs.attackDamage);
                }
            }
        }

        public void ActivateInvulnerability(float time)
        {
            StartCoroutine(StartInvulnerabilityTimer(time));
        }

        public string GetActiveCharacterName()
        {
            return _characterManager.ActiveCharacter.CharacterName;
        }

    #endregion

        void SetComponents()
        {
            _rb = GetComponent<Rigidbody2D>();
            _characterManager = new CharactersManager(_charactersModels, startingCharacterIndex);
            _playerInput = GetComponent<PlayerInput>();
        }
        
        public void ChangeActionMapTo(string inputMap)
        {
            _playerInput.SwitchCurrentActionMap(inputMap);
        }

        #region Debug

        void OnDrawGizmosSelected()
        {
            DrawAttackSphereGizmo();
            DrawInteractionSphereGizmo();
        }

        void DrawAttackSphereGizmo()
        {
            Vector2 attackDistance = CalculateAttackOffset();
        
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackDistance, attackRange);
        }
        
        void DrawInteractionSphereGizmo()
        {
            Vector2 origin = transform.position;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(origin, interactionDistance);
        }
        
        #endregion
    }
}