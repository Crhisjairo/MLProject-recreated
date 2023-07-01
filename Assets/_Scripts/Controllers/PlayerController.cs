using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Characters;
using _Scripts.Controllers.PlayerStates;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


namespace _Scripts.Controllers
{
    public delegate void OnPauseHandler(bool isStopped);
 
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { private set; get; }
        
        public event OnPauseHandler OnPauseHandler;
        
        [SerializeField] private PlayerInput PlayerInput;
        public IPlayerState currentState;
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

        public bool IsInvulnerable { private set; get; }
        public float _defaultInvulnerabilityTime = 0.7f;
        public int characterIndex = 0;
        public float currentSpeed;

        public Rigidbody2D _currentRb;
        [FormerlySerializedAs("_currentPlayerSpecs")] public CharacterSpecs currentCharacterSpecs;
        public Animator _currentAnimator;
        public ParticleSystem _currentParticleSystem;
        public SpriteRenderer _currentSpriteRenderer;

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

        PlayerInput = GetComponent<PlayerInput>();
        
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

    void Start()
    {
        MoveIndexCharacter(characterIndex);
        
        //currentState = new InGame(this);
    }

    private void FixedUpdate()
    {
        //Movemos al personaje
        _currentRb.MovePosition(_currentRb.position + movement * currentSpeed * Time.fixedDeltaTime);
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
        // currentCharacterSpecs = currentCharacter.GetCharacterSpecs();
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
        currentState.HandleDpadInput(inputContext);
    }
    
    public void Run(InputAction.CallbackContext inputContext)
    {
        currentState.HandleRunInput(inputContext);
    }
    
    public void Attack(InputAction.CallbackContext context)
    {
        currentState.HandleAttackInput(context);
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        currentState.HandleInventoryInput(context);
    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        currentState.HandlePauseInput(context);
    }
    
    public void ChangeCharacter(InputAction.CallbackContext context)
    {
        currentState.HandleChangeCharacterInput(context);
    }

    public void TogglePauseGame()
    {
        GameInPause = !GameInPause;

        Time.timeScale = GameInPause ? 0 : 1;
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
        OnPauseHandler?.Invoke(isPaused);
    }

    private void OnDisable()
    {
        Instance = null;
    }

    #region Calculations

    public void SaveLookingDirection()
    {
        //Guardamos la direccion en la que miramos segun el axis
        if (movement.x > 0.01f)
        {
            _frontDirection = Vector2.right;
        }
        if (movement.x < -0.01f)
        {
            _frontDirection = Vector2.left;
        }
        if (movement.y > 0.01f)
        {
            _frontDirection = Vector2.up;
        }
        if (movement.y < -0.01f)
        {
            _frontDirection = Vector2.down;
        }
    }

    public void SetAnimationByIdleDirection()
    {
        if (_frontDirection.x > 0.01f)
        {
            _currentAnimator.SetFloat("LastHorizontal", 1);
            _currentAnimator.SetFloat("LastVertical", 0);
        }
        if (_frontDirection.x < -0.01f)
        {
            _currentAnimator.SetFloat("LastHorizontal", -1);
            _currentAnimator.SetFloat("LastVertical", 0);
        }
        if (_frontDirection.y > 0.01f)
        {
            _currentAnimator.SetFloat("LastVertical", 1);
            _currentAnimator.SetFloat("LastHorizontal", 0);
        }
        if (_frontDirection.y < -0.01f)
        {
            _currentAnimator.SetFloat("LastVertical", -1);
            _currentAnimator.SetFloat("LastHorizontal", 0);
        }
    }

    public void SetAnimationByMovingDirection()
    {
        //Animacion
        _currentAnimator.SetFloat("Horizontal", movement.x);
        _currentAnimator.SetFloat("Vertical", movement.y);
        _currentAnimator.SetFloat("Speed", movement.sqrMagnitude); //La velocidad de movimiento
    }
    
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
    
    public void CheckFrontInteraction(InputAction.CallbackContext inputContext)
    {
        currentState.HandleInteractInput(inputContext);
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
