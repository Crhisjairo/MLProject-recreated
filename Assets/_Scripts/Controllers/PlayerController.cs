using System;
using System.Collections;
using _Scripts.Controllers.Characters;
using _Scripts.Controllers.Enemies.Interfaces;
using _Scripts.Controllers.Interfaces;
using _Scripts.Enums;
using _Scripts.GameManagerSystem;
using _Scripts.GameManagerSystem.Models;
using _Scripts.Shared.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] LayerMask enemyLayers;
        [SerializeField] LayerMask interactorsLayers;
        
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

        public UnityEvent onPlayerDied;

        public UnityEvent onPlayerIdle;
        public UnityEvent onPlayerMoved;
        
        [Tooltip("Set to false to use unity's inspector values.")]
        public bool useSaveData = true;
        
        private bool IsInvulnerable { set; get; }

        private CharactersManager _characterManager;
        private PlayerInput _playerInput;

        private Rigidbody2D _rb;

        private const float diagonalLimiter = 0.99f; // 0.7 default
        private Vector2 movement;
        private Vector2 _impulseDirection;
        
        private float currentSpeed;

        private bool _inImpulse = false;
        private const float ImpulseTime = .15f;

        [SerializeField] private bool isAbleToRun = false;
        [SerializeField] private bool isAbleToAttack = false;
        [SerializeField] private bool isAbleToOpenInventory = false;

        private SaveDataSystem _saveDataSystem;
        
        void Awake()
        {
            SetComponents();
        }
        
        void Start()
        {
            _saveDataSystem = SaveDataSystem.Instance;
            
            LoadSavedPlayerData();

            currentSpeed = _characterManager.ActiveCharacter.GetSpeed();
            
            UpdaterAllPlayerUI();
        }
        
        void FixedUpdate()
        {
            _rb.MovePosition(_rb.position + movement * (currentSpeed * Time.fixedDeltaTime));

            if (_inImpulse)
            {
                _rb.AddForce(_impulseDirection, ForceMode2D.Force);
            }
        }

        private void LoadSavedPlayerData()
        {
            if (!useSaveData)
                return;
            
            var loadedData = _saveDataSystem.GetPlayerSaveDataSelected();

            isAbleToRun = loadedData.isAbleToRun;
            isAbleToAttack = loadedData.isAbleToAttack;
            isAbleToOpenInventory = loadedData.isAbleToOpenInventory;

            if (!loadedData.isAutoSaved)
            {
                var newPos = new Vector3(
                    loadedData.lastPlayerPosition.x, loadedData.lastPlayerPosition.y, transform.position.z
                    );
                
                transform.position = newPos;
            }
            
            //TODO: Set all values given by the save data system!
            
            _characterManager.LoadCharacterSaveDatas(loadedData.CharacterSaveData);
            _characterManager.CurrentCoins = loadedData.coinsAmount;

        }

        public PlayerSaveData BuildPlayerSaveData(bool includePlayerPosition)
        {
            var saveData = _saveDataSystem.GetPlayerSaveDataSelected();

            saveData.isAbleToRun = isAbleToRun;
            saveData.isAbleToAttack = isAbleToAttack;
            saveData.isAbleToOpenInventory = isAbleToOpenInventory;
            
            // NOTE: Player position is not saved.
            saveData.lastPlayerPosition = includePlayerPosition 
                ? new PlayerPosition(transform.position.x, transform.position.y) 
                : null;

            saveData.CharacterSaveData = _characterManager.BuildCharacterSaveDatas();
            saveData.coinsAmount = _characterManager.CurrentCoins;
            
            return saveData;
        }

        /// <summary>
        /// This method must be called JUST WHEN NECESSARY, because it updates all player UI elements.
        /// </summary>
        public void UpdaterAllPlayerUI()
        {
            onMaxLifeUpdate?.Invoke(_characterManager.ActiveCharacter.GetMaxLife());
            onLifeGetted?.Invoke(_characterManager.ActiveCharacter.GetCurrenLife());
            onCoinsUpdate?.Invoke(_characterManager.CurrentCoins);
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

        /// <summary>
        /// Check if item can be added to inventory. If not, the item will not be added.
        /// </summary>
        /// <param name="item">Item to add to inventory.</param>
        /// <returns>If item was added successfully</returns>
        public bool TryToAddItemToInventory(GameObject item)
        {
            return true;
        }
        
        public void StartRunning(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.performed || !isAbleToRun)
                return;
            
            currentSpeed = _characterManager.ActiveCharacter.GetRunningSpeed();
            _characterManager.ActiveAnimator.SetBool(CharacterAnimationStates.Running.ToString(), true);
        }

        public void StopRunning(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.canceled || !isAbleToRun)
                return;
            
            currentSpeed = _characterManager.ActiveCharacter.GetSpeed();
            _characterManager.ActiveAnimator.SetBool(CharacterAnimationStates.Running.ToString(), false);
        }
        
        public void Attack(InputAction.CallbackContext context)
        {
            if (!context.performed || !isAbleToAttack)
                return;

            if (Time.time >= _nextAttackTime)
            {
                _characterManager.ActiveAnimator.SetTrigger("Attack");
                
                Vector2 attackOffset = CalculateAttackOffset();

                _characterManager.ActiveParticleSystem.gameObject.transform.position = new Vector3(attackOffset.x, attackOffset.y, -1);
                _characterManager.ActiveParticleSystem.Play();
                _characterManager.ActiveCharacter.PlaySoundSfx(SoundsFX.Attack);
                
                AttackEnnemiesOnOverlapCircle(attackOffset);
                
                _nextAttackTime = Time.time + 1f / attackRate;
            }
        }

        public void ReceiveDamage(Vector2 impulseDirection, int damageAmount)
        {
            // apply the impulse to the rigid body for an amount of time.
            
            //Verificamos que no seamos invulnerables
            if (IsInvulnerable)
            {
                return;
            }

            _characterManager.ActiveCharacter.PlaySoundSfx(SoundsFX.Damaged);
            
            StartCoroutine(ActivateImpulseCounter(impulseDirection));
            
            //TODO: create a CameraController to ShakeCamera(1.5f, 0.1f); onDamageTaken
            
            _characterManager.ActiveAnimator.SetTrigger(CharacterAnimationStates.Damaged.ToString());
            
            ActivateInvulnerability(_defaultInvulnerabilityTime);
            
            _characterManager.ActiveCharacter.TakeDamage(damageAmount);
            
            onDamageTaken?.Invoke(_characterManager.ActiveCharacter.GetCurrenLife());

            
            Debug.Log("Current life: " + _characterManager.ActiveCharacter.GetCurrenLife());
            
            //TODO: Temporal. Add a die screen
            if (_characterManager.ActiveCharacter.GetCurrenLife() <= 0)
            {
                onPlayerDied?.Invoke();
                PlayAnimation(CharacterAnimationStates.Tired.ToString());
                _playerInput.SwitchCurrentActionMap(PlayerActionMaps.InCinematic.ToString());
            }
        }

        private IEnumerator ActivateImpulseCounter(Vector2 impulseDirection)
        {
            _inImpulse = true;
            _impulseDirection = impulseDirection;
            
            yield return new WaitForSeconds(ImpulseTime);
            
            _inImpulse = false;
            
            _impulseDirection = new Vector2(0,0);
        }
        
        public void TakeLife(int lifeAmount)
        {
            _characterManager.ActiveCharacter.TakeLife(lifeAmount);
            
            onLifeGetted?.Invoke(_characterManager.ActiveCharacter.GetCurrenLife());
        }

        public void TakeExtraHeartSlot()
        {
            _characterManager.ActiveCharacter.AddExtraHeartSlot();
            
            onMaxLifeUpdate?.Invoke(_characterManager.ActiveCharacter.GetMaxLife());
        }

        public void AddHeart(int amount)
        {
            _characterManager.ActiveCharacter.TakeLife(amount);
        }
        
        public void AddCoins(int amount)
        {
            _characterManager.AddCoins(amount);
            
            onCoinsUpdate?.Invoke(_characterManager.CurrentCoins);
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
            Collider2D collider = Physics2D.OverlapCircle(origin, interactionDistance, interactorsLayers);

            Component interactuableComponent;

            if (collider)
            {
                collider.gameObject.TryGetComponent(typeof(IInteractable), out interactuableComponent);

                
                Debug.Log(collider.gameObject.name);
                
                if (interactuableComponent)
                {
                    var interactable = (IInteractable) interactuableComponent ;
                    interactable.Interact(this);
                }
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
            //Nos aseguramos de quedarnos blancos xd
            _characterManager.ActiveSpriteRenderer.color = Color.white;
            
            IsInvulnerable = false;
            
            
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
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackOffset, attackRange, enemyLayers);
            
            foreach (var enemyCollider in hitEnemies)
            {
                if (!enemyCollider.isTrigger)
                {
                    Debug.Log("Enemy hit: " + enemyCollider.name);

                    if (enemyCollider.TryGetComponent(typeof(IAttackable), out var component))
                    {
                        var attackable = component as IAttackable;
                        int attackDamage = _characterManager.ActiveCharacter.GetAttackDamage();
                        
                        // Vector opuesto para el enemigo
                        Vector2 enemyImpulseDir = enemyCollider.transform.position - transform.position;
                        enemyImpulseDir = enemyImpulseDir.normalized * _characterManager.ActiveCharacter.GetForceImpulse();
                        
                        Debug.Log(enemyImpulseDir);
                        
                        attackable?.ReceiveDamage(enemyImpulseDir, attackDamage);
                    }
                }
            }
        }

        public void ActivateInvulnerability(float time)
        {
            StartCoroutine(StartInvulnerabilityTimer(time));
        }

        public string GetActiveCharacterName()
        {
            return _characterManager.ActiveCharacter.GetCharacterName().ToString();
        }

        public void SetIsAbleToRun(bool canRun)
        {
            isAbleToRun = canRun;
        }

        
        public void SetIsAbleToAttack(bool canAttack)
        {
            isAbleToAttack = canAttack;
        }
        
        public void SetIsAbleToOpenInventory(bool canOpenInventory)
        {
            isAbleToOpenInventory = canOpenInventory;
        }
        
    #endregion

        void SetComponents()
        {
            _rb = GetComponent<Rigidbody2D>();
            _characterManager = new CharactersManager(_charactersModels, startingCharacterIndex); // TODO: maybe move this line on Start method.
            _playerInput = GetComponent<PlayerInput>();
        }

        public void ChangeActionMapToString(string actionMapStr)
        {
            _playerInput.SwitchCurrentActionMap(actionMapStr);
            Debug.Log(_playerInput.currentActionMap);
        }

        public PlayerActionMaps GetCurrentActionMap()
        {
            return Enum.Parse<PlayerActionMaps>(_playerInput.currentActionMap.name);
        }
        
        public void ChangeActionMapTo(PlayerActionMaps inputMap)
        {
            _playerInput.SwitchCurrentActionMap(inputMap.ToString());
            Debug.Log(_playerInput.currentActionMap);
        }

        #region Debug

        /// <summary>
        /// ONLY FOR DEBUG.
        /// </summary>
        public void ReceiveDamageWithoutImpulseDebug(int damageAmount)
        {
            ReceiveDamage(new Vector2(), damageAmount);
        }
        
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