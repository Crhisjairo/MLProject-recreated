using System;
using System.Collections;
using _Scripts.Characters;
using _Scripts.Enums;
using _Scripts.Interactuable;
using _Scripts.Utils;
using UnityEditor.VersionControl;
using UnityEngine;
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
        public float attackRange = 0.25f; // TODO maybe move that to Character class
        public Vector2 distanceAttackRange; // TODO maybe move that to Character class
        public float _nextAttackTime; // TODO maybe move that to Character class
        public float interactionDistance = 0.5f;
        
        bool IsInvulnerable { set; get; }

        CharactersManager _characterManager;
        PlayerInput _playerInput;

        Rigidbody2D _rb;

        float diagonalLimiter = 0.9f; // 0.7 default
        Vector2 movement;
        
        float currentSpeed;
        
        void Awake()
        {
            SetComponents();
        }
        
        void Start()
        {
            currentSpeed = _characterManager.ActiveCharacter.GetSpeed();
        }
        
        void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + movement * (currentSpeed * Time.fixedDeltaTime));
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
            activeCharacter.SetAnimationByMovingDirection(movement);
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
            
            //Movemos la câmara
            ShakeCamera(1.5f, 0.1f);
            //Animamos al personaje
            _characterManager.ActiveAnimator.SetTrigger("Damaged");
            //Activamos un countdown para la vulnerabilidad
            ActivateInvulnerability(_defaultInvulnerabilityTime);
            //Hacemos daño
            _characterManager.ActiveCharacter.TakeDamage(damageAmount); //Bajamos la vida del jugador
            // TODO Notify HUD
        }
        
        public void TakeLife(int lifeAmount)
        {
            _characterManager.ActiveCharacter.TakeLife(lifeAmount); //Damos vida desde el otro script
            // TODO Notify HUD
        }
        public void AddCoins(int amount)
        {
            _characterManager.ActiveCharacter.AddCoins(amount);
            // TODO Notify HUD
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
            RaycastHit2D hit = Physics2D.Raycast(origin, lookingDirection, interactionDistance, ~LayerMask.GetMask("Player"));
            
            if (hit)
            {
                IInteractuable interactuable = hit.collider.gameObject.GetComponent<IInteractuable>();
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
            Vector3 position = _characterManager.ActiveCharacter.transform.position;
            Vector2 lookingDirection = _characterManager.ActiveCharacter.GetLookingDirection();
            
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

        #region DebugRegion

        private void OnDrawGizmosSelected()
        {
            //Vector2 attackDistance = CalculateAttackOffset();
        
            //Gizmos.DrawWireSphere(attackDistance, attackRange);
        
            //Gizmos.DrawWireSphere(_characterManager.ActiveCharacter.transform.position, 1.5f);
            
            // Interact ray
            Vector2 origin = transform.position;
            Vector2 lookingDirection = Vector2.down;

            if (_characterManager is not null)
            {
                lookingDirection = _characterManager.ActiveCharacter.GetLookingDirection();
            }
            
            Debug.DrawRay(origin, lookingDirection * interactionDistance, Color.red);
        }

        #endregion
    }
}