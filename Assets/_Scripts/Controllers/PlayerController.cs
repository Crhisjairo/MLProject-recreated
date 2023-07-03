using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Characters;
using _Scripts.Enums;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace _Scripts.Controllers
{
    public delegate void OnPauseHandler(bool isStopped);

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { private set; get; }
        
        private PlayerInput PlayerInput;
        InputActionMap _lastActionMap, _currentActionMap;
        
        //[SerializeField] public HUDManager _hudManager;
        //[SerializeField] public InventoryManager _inventoryManager;
        //[SerializeField] public CameraManager cameraManager;

        [SerializeField] private GameObject[] _charactersModels;
        private List<GameObject> _playerCharactersGameObjects = new List<GameObject>();
        private GameObject _currentCharacterGameObject;
        
        public List<Character> _playerCharacters = new List<Character>();
        public Character currentCharacter;
        
        public bool GameInPause { get; set; } = false;

        public Vector2 movement;
        public Vector2 _frontDirection = Vector2.down;

        public float attackRate = 2f;
        public float attackRange = 0.25f;
        public Vector2 distanceAttackRange;
        [SerializeField] public LayerMask _enemyLayers;
        public float _nextAttackTime;
        
        public float diagonalLimiter = 0.9f; // 0.7 default

        private bool IsInvulnerable { set; get; }
        private float _defaultInvulnerabilityTime = 0.7f;
        private int characterIndex = 0;
        private float currentSpeed;

        private Rigidbody2D _rb;
        private CharacterSpecs currentCharacterSpecs;
        private Animator _currentAnimator;
        private ParticleSystem _currentParticleSystem;
        private SpriteRenderer _currentSpriteRenderer;

        private void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
            }
            else
            {
                throw new Exception("Cannot have two PlayerController instances.");
            }

            SetComponents();
            
            //Load character en funciôn de un save data. Por ahora, solo cargarê a Twi
            foreach (var model in _charactersModels)
            {
                _playerCharacters.Add(model.GetComponent<Character>());
                _playerCharactersGameObjects.Add(model);
            }
            
            _currentCharacterGameObject = _playerCharactersGameObjects[characterIndex];
            currentCharacter = _currentCharacterGameObject.GetComponent<Character>();
            
            SetCurrentCharacterComponents();
            DisableOtherCharacters();
        }

        void SetComponents()
        {
            PlayerInput = GetComponent<PlayerInput>();
            _rb = GetComponent<Rigidbody2D>();
        }
        void Start()
    {
        MoveIndexCharacter(characterIndex);
        
        //currentState = new InGame(this);
    }

    private void FixedUpdate()
    {
        //Movemos al personaje
        _rb.MovePosition(_rb.position + movement * currentSpeed * Time.fixedDeltaTime);
    }

    public void SetCharactersSpecs(CharacterSpecs[] characterSpecs)
    {
        for (var i = 0; i < _playerCharacters.Count; i++)
        {
            //var newSpecs = characterSpecs.FirstOrDefault(x => x.Id == _playerCharacters[i].GetCharacterSpecs().Id);
            
            //_playerCharacters[i].SetCharacterSpecs(newSpecs);
        }

        currentCharacter = _playerCharacters[characterIndex];
        
        SetCurrentCharacterComponents();
    }

    public CharacterSpecs[] GetCharacterSpecs()
    {
        //return _playerCharacters.Select(x => x.GetCharacterSpecs()).ToArray();
        return null;

    }

    private void SetCurrentCharacterComponents()
    {
        // _currentRb = currentCharacter.GetRigidBody();
        //currentCharacterSpecs = currentCharacter.GetCharacterSpecs();
        // _currentAnimator = currentCharacter.GetAnimator();
        // _currentParticleSystem = currentCharacter.GetParticleSystem();
        // _currentSpriteRenderer = currentCharacter.GetSpriteRenderer();
        
        // currentSpeed = currentCharacter.GetCharacterSpecs().speed;
    }
    
    private void DisableOtherCharacters()
    {
        //Solo dejamos el primer personaje activo
        for (int i = 0; i < _playerCharactersGameObjects.Count; i++)
        {
            if (i == characterIndex)
            {
                continue;
            }
            
            _playerCharactersGameObjects[i].SetActive(false);
        }
    }
    
    public void Move(InputAction.CallbackContext inputContext)
    {
        Vector2 inputMovement = inputContext.ReadValue<Vector2>();
        
        movement.x = 0; 
        movement.y = 0;
            
        if (inputMovement.x != 0 && inputMovement.y != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            inputMovement.x *= diagonalLimiter;
            inputMovement.y *= diagonalLimiter;
        }
            
        movement.x = inputMovement.x;
        movement.y = inputMovement.y; 
        
        //Guardamos la direcciôn solo en el modo RPG
        currentCharacter.SetLookingDirection(movement);
        currentCharacter.SetAnimationByIdleDirection(movement);
        currentCharacter.SetAnimationByMovingDirection(movement);
    }
    
    public void Run(InputAction.CallbackContext inputContext)
    {
        if (!(inputContext.performed || inputContext.canceled))
        {
            return;
        }
        
        bool running = inputContext.performed;
            
        //Antes de movernos, verificamos que no estemos en dialogo
        if (running)
        {
            currentSpeed = currentCharacterSpecs.GetRunningSpeed();
            _currentAnimator.SetBool(CharacterAnimationStates.Running.ToString(), true);
        }
        else
        { 
            currentSpeed = currentCharacterSpecs.GetSpeed();
            _currentAnimator.SetBool(CharacterAnimationStates.Running.ToString(), false);
        }
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (Time.time >= _nextAttackTime)
        {
            //Animamos al personaje
            _currentAnimator.SetTrigger("Attack");
            
            //Calculamos la direccion del ataque
            //Creamos la direcciôn y la distance de ataque relativa al personaje
            Vector2 attackOffset = CalculateAttackOffset();

            //Ponemos las particulas en la misma posiciôn de ataque
            _currentParticleSystem.gameObject.transform.position = new Vector3(attackOffset.x, attackOffset.y, -1);
            _currentParticleSystem.Play();
            
            //Reproducimos el sonido
            currentCharacter.PlaySoundSfx(CharacterSfx.UnicornAttackSfx);
            
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

    public void PauseGame(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
            
        TogglePauseGame();

        //TODO Change InputMap and notify HUD
            
    }
    
    public void ChangeCharacter(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
            
        //Para cambiar de personaje
        var sideToSwitch = context.ReadValue<float>();
            
        ChangeCharacterTo(sideToSwitch);
    }
    
    public void CheckFrontInteraction(InputAction.CallbackContext inputContext)
    {
        if (!inputContext.performed)
            return;

        //Origen de donde parte el rayo
        Vector2 origin = currentCharacter.transform.position;
        float distance = 1.5f; //distancia maxima del rayo
            
        //TODO
        Collider2D hit = Physics2D.OverlapCircle(origin, 1f, 3);

        Debug.Log(hit.name);
         
        
        if (hit)
        {
            //PlayerController.InteractWith(hit);
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

    public void TogglePauseGame()
    {
        //TODO Move this to GameManager
        /**
        GameInPause = !GameInPause;

        Time.timeScale = GameInPause ? 0 : 1;
        **/
    }

    public void ChangeCharacterTo(float input)
    {
        //Verificamos si hay solo un personaje, si hay un solo personaje, characterIndex siempre es 0
        if (_playerCharactersGameObjects.Count == 1)
        {
            MoveIndexCharacter(0);
            return;
        }
        
        if (input == -1) //Cambiamos a la izquierda
        {
            //Si sobre pasamos el index, reiniciamos
            if (characterIndex >= _playerCharactersGameObjects.Count - 1)
            {
                characterIndex = 0;
            }
            else //sino, sumamos
            {
                //Suma 1 al index del personaje actual
                characterIndex++; 
            }
        }

        if (input == 1) //Cambiamos a la derecha
        {
            //Si sobre pasamos el index, reiniciamos
            if (characterIndex <= 0)
            {
                characterIndex = _playerCharactersGameObjects.Count - 1;
            }
            else //sino, sumamos
            {
                //Suma 1 al index del personaje actual
                characterIndex--; 
            }
            
        }
        
        MoveIndexCharacter(characterIndex);
    }

    private void MoveIndexCharacter(int index)
    {
        //Guarda la posicion del personaje actual
        Transform charTrans = currentCharacter.transform;
            
        //Deshabilita el personaje actial
        _currentCharacterGameObject.SetActive(false);
        //Cambia el personaje actual
        _currentCharacterGameObject = _playerCharactersGameObjects[index];
        //Habilita el nuevo personaje y le da su posicion
        _currentCharacterGameObject.SetActive(true);
        currentCharacter = _currentCharacterGameObject.GetComponent<Character>();

        SetCurrentCharacterComponents();

        currentCharacter.transform.SetPositionAndRotation(charTrans.position, charTrans.rotation);

        //Le decimos a los suscriptores de actualizar el jugador
        //En este caso a OnScreenControls, HUDManager o Flying Game Manager
        //_hudManager.UpdateCharacterSpecsOnHUD();

        // cameraManager.FollowAt(currentCharacter.transform);
    }
    
    /**
    public bool TryToAddItemToInventory(ItemData itemData)
    {
        //si no se pudo agregar, return
        if (InventoryManager.Instance.AddItem(itemData))
        {
            return false;
        }
     
        _hudManager.UpdateItemsOnInventory();
        return true;
    }
    **/
    
    public void TakeLife(int lifeAmount)
    {
        currentCharacterSpecs.TakeLife(lifeAmount); //Damos vida desde el otro script
        //_hudManager.UpdatePlayerLifeOnly(); //Actualizamos el HUD
    }
    
    public void performOnPause(bool isPaused)
    {
        // TODO Make with Observer Patterns
    }

    private void OnDisable()
    {
        Instance = null;
    }

    #region Calculations

        /// <summary>
    /// Calcula la distancia entre el jugador y la zona de ataque en base a Distance Attack Range.
    /// Si el radio de ataque aumenta, la distancia de ataque tambiên deberîa aumentar
    /// </summary>
    /// <returns>Posicion del centro de ataque relativo al personaje</returns>
    public Vector2 CalculateAttackOffset()
    {
        Vector3 position = currentCharacter.transform.position;
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
        _currentSpriteRenderer.material.color = Color.white;
        
        //GameManager.Instance.CanPlayerInteract = true;
    }
    
    private IEnumerator StartFlashing()
    {
        var spriteRenderer = _currentSpriteRenderer;
        
        while (true)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.09f);
            
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.09f);
        }
    }
    
    private void ShakeCamera(float intensity, float time)
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

    #region GettersAndSetters

    public void AddCoins(int amount)
    {
        currentCharacterSpecs.AddCoins(amount);
        // _hudManager.UpdateCoinAmount(CharacterSpecs.currentCoins);
    }

    public string GetPlayerName()
    {
        return currentCharacterSpecs.characterName;
    }
    
    public bool HasInventoryEmptySlots()
    {
        //return _gameManager.inventoryManager.HasEmptySlot();
        return false;
    }

    public int GetMaxLife()
    {
        // return currentCharacterSpecs.maxLife;
        return 0;
    }
    
    public int GetCurrentLife()
    {
        // return currentCharacterSpecs.currentLife;
        return 0;
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
        _currentAnimator.SetTrigger("Damaged");
        //Activamos un countdown para la vulnerabilidad
        ActivateInvulnerability(_defaultInvulnerabilityTime);
        //Hacemos daño
        currentCharacterSpecs.TakeDamage(damageAmount); //Bajamos la vida del jugador
        // _hudManager.UpdatePlayerLifeOnly(); //Actualizamos el HUD
    }
    
    public CharacterSpecs GetPlayerSpecs()
    {
        return currentCharacterSpecs;
    }

    public void SetPlayerSpecs(CharacterSpecs newCharacterSpecs)
    {
        currentCharacterSpecs = newCharacterSpecs;
    }

    public void PlayAnimation(string animName)
    {
        //_currentAnimator.Play(animName);
        _currentAnimator.SetTrigger(animName);
    }

    #endregion

    #region DebugRegion

    private void OnDrawGizmosSelected()
    {
        Vector2 attackDistance = CalculateAttackOffset();
        
        //Gizmos.DrawWireSphere(attackDistance, attackRange);
        
        Gizmos.DrawWireSphere(currentCharacter.transform.position, 1.5f);
    }

    #endregion
    }
}
