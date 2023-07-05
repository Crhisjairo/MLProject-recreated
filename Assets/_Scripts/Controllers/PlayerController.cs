using System;
using System.Collections;
using _Scripts.Characters;
using _Scripts.Enums;
using _Scripts.Utils;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] public LayerMask _enemyLayers;
        [SerializeField] private GameObject[] _charactersModels;
        [SerializeField] int startingCharacterIndex = 0;

        [SerializeField] private float _defaultInvulnerabilityTime = 0.7f;

        public float attackRate = 2f; // TODO maybe move that to Character class
        public float attackRange = 0.25f; // TODO maybe move that to Character class
        public Vector2 distanceAttackRange; // TODO maybe move that to Character class
        public float _nextAttackTime; // TODO maybe move that to Character class
        
        bool IsInvulnerable { set; get; }

        CharactersUtil _characterUtil;

        Rigidbody2D _rb;

        float diagonalLimiter = 0.9f; // 0.7 default
        Vector2 movement;
        Vector2 _frontDirection = Vector2.down;
        
        float currentSpeed;
        
        void Awake()
        {
            SetComponents();
        }
        
        void Start()
        {
            currentSpeed = _characterUtil.ActiveCharacter.GetSpeed();
        }
        
        void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + movement * (currentSpeed * Time.fixedDeltaTime));
        }
        
        public void Move(InputAction.CallbackContext inputContext)
        {
            Vector2 inputMovement = inputContext.ReadValue<Vector2>();
            
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
            
            var activeCharacter = _characterUtil.ActiveCharacter;
            
            activeCharacter.SetLookingDirection(movement);
            activeCharacter.SetAnimationByIdleDirection(movement);
            activeCharacter.SetAnimationByMovingDirection(movement);
        }
        
        public void Run(InputAction.CallbackContext inputContext)
        {
            if (!(inputContext.performed || inputContext.canceled))
            {
                return;
            }

            var activeCharacter = _characterUtil.ActiveCharacter;
            bool running = inputContext.performed;
                
            //Antes de movernos, verificamos que no estemos en dialogo
            if (running)
            {
                currentSpeed = activeCharacter.GetRunningSpeed();
                _characterUtil.ActiveAnimator.SetBool(CharacterAnimationStates.Running.ToString(), true);
            }
            else
            { 
                currentSpeed = activeCharacter.GetSpeed();
                _characterUtil.ActiveAnimator.SetBool(CharacterAnimationStates.Running.ToString(), false);
            }
        }
        
        public void Attack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (Time.time >= _nextAttackTime)
            {
                //Animamos al personaje
                _characterUtil.ActiveAnimator.SetTrigger("Attack");
                
                //Calculamos la direccion del ataque
                //Creamos la direcciôn y la distance de ataque relativa al personaje
                Vector2 attackOffset = CalculateAttackOffset();

                //Ponemos las particulas en la misma posiciôn de ataque
                _characterUtil.ActiveParticleSystem.gameObject.transform.position = new Vector3(attackOffset.x, attackOffset.y, -1);
                _characterUtil.ActiveParticleSystem.Play();
                
                //Reproducimos el sonido
                _characterUtil.ActiveCharacter.PlaySoundSfx(CharacterSfx.UnicornAttackSfx);
                
                //Atacamos a los enemigos
                AttackEnnemiesOnOverlapCircle(attackOffset);
                
                //Recalculamos el tiempo segun el attackRate
                _nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        public void OpenInventory(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            // TODO change InputMap, pause game and open inventory
            // _inventoryManager.ShowInventory(PlayerController.GameInPause);
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
            _characterUtil.ActiveAnimator.SetTrigger("Damaged");
            //Activamos un countdown para la vulnerabilidad
            ActivateInvulnerability(_defaultInvulnerabilityTime);
            //Hacemos daño
            _characterUtil.ActiveCharacter.TakeDamage(damageAmount); //Bajamos la vida del jugador
            // TODO Notify HUD
        }

        
        public void TakeLife(int lifeAmount)
        {
            _characterUtil.ActiveCharacter.TakeLife(lifeAmount); //Damos vida desde el otro script
            // TODO Notify HUD
        }
        public void AddCoins(int amount)
        {
            _characterUtil.ActiveCharacter.AddCoins(amount);
            // TODO Notify HUD
        }

        public void PlayAnimation(string animName)
        {
            //_characterUtil.ActiveAnimator.Play(animName);
            _characterUtil.ActiveAnimator.SetTrigger(animName);
        }
        
        public void CheckFrontInteraction(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.performed)
                return;

            //Origen de donde parte el rayo
            Vector2 origin = _characterUtil.ActiveCharacter.transform.position;
            float distance = 1.5f; //distancia maxima del rayo
                
            //TODO
            Collider2D hit = Physics2D.OverlapCircle(origin, 1f, 3);

            Debug.Log(hit.name);
            
            if (hit)
            {
                //InteractWith(hit);
            }
            
            Debug.DrawRay(origin, _frontDirection * distance, Color.red);
        }
        
        public void InteractWith(RaycastHit2D hit)
        {
            if (hit.collider.gameObject.tag.Equals("NPC")  )
            {
                //Si es un NPC (que es un Interactuable), cambiamos de estado
                //Agregar condiciones con tag para saber con quiên se debe interactuar
                // IInteractuable interactuable = hit.collider.GetComponent<NPCController>();
                // interactuable.Interact(this);
            }
            
            if (hit.collider.CompareTag("Sign"))
            {
                //LLamamos al dialogueTrigger del letrero
                // IInteractuable interactuable = hit.collider.GetComponent<Sign>();
                // interactuable.Interact(this);
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
            Vector3 position = _characterUtil.ActiveCharacter.transform.position;
            Vector2 relDirection =  _frontDirection.normalized + new Vector2(position.x, position.y);

            if (_frontDirection.x > 0.01) //Derecha
            {
                relDirection.x += distanceAttackRange.x;
            }

            if (_frontDirection.x < -0.01) //Izquierda
            {
                relDirection.x -= distanceAttackRange.x;
            }
            
            if (_frontDirection.y > 0.01) //Derecha
            {
                relDirection.y += distanceAttackRange.y;
            }

            if (_frontDirection.y < -0.01) //Izquierda
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
            _characterUtil.ActiveSpriteRenderer.material.color = Color.white;
            
            //GameManager.Instance.CanPlayerInteract = true;
        }
        
        IEnumerator StartFlashing()
        {
            var spriteRenderer = _characterUtil.ActiveSpriteRenderer;
            
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
            _characterUtil = new CharactersUtil(_charactersModels, 0);
        }

        #region DebugRegion

        private void OnDrawGizmosSelected()
        {
            Vector2 attackDistance = CalculateAttackOffset();
        
            //Gizmos.DrawWireSphere(attackDistance, attackRange);
        
            Gizmos.DrawWireSphere(_characterUtil.ActiveCharacter.transform.position, 1.5f);
        }

        #endregion
    }
}