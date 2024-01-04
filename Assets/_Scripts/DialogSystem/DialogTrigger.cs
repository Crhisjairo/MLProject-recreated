using System;
using System.Collections;
using _Scripts.Controllers;
using _Scripts.Enums;
using _Scripts.SoundsManagers;
using _Scripts.Utils;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.DialogSystem
{
    /// <summary>
    /// Checks the correct dialog to set on DialogManager.
    /// </summary>
    public class DialogTrigger : MonoBehaviour
    {
        [Header("Controller references")]
        [SerializeField] DialogController dialogController;
        [SerializeField] CinemachineVirtualCamera cam;
        [Space(10)]
        
        
        
        [Header("Dialog attributes")]
        public DialogsSet dialogs;

        public bool _autoNextDialog; // TODO: agregar un temporizador al autoNextDialog.
        public float _timeToWaitAutoNextDialog = 1f;
        
        public Sound talkingAudio;
        public bool randomizeTalkingPitch; 
        
        public float zoomAmountOnDialog = 0;
        public float speedZoom = 0.5f;
        
        [Range(0f, 0.3f)]
        public float textTypingDelay = 0.1f;
        
        [Space(10)]
        
        [Header("Events")]
        [SerializeField] private UnityEvent onAbleToInteract; 
        [SerializeField] private UnityEvent onUnableToInteract;    
        
        [Space(10)]
        
        private PlayerActionMaps inDialogActionMapName = PlayerActionMaps.InDialog;

        [SerializeField] private SpriteRenderer interactSprite;
        [SerializeField] private Animator _animator;

        bool _isFirstInteraction = true;
        bool _isDialogueTriggered;
        bool _canInteract;
        
        bool _playerInRange = false;

        float _currentCamZoom = 0f;

        private void Awake()
        {
            CheckForOptionalComponents();

            if (_animator != null)
                _animator.enabled = false;
            if(interactSprite != null)
                interactSprite.enabled = false;
        }

        void Start()
        {
            _currentCamZoom = cam.m_Lens.OrthographicSize;
        }

        public void ForceStartDialogs(PlayerController interactor)
        {
            _playerInRange = true;
            
            TryToStartDialogs(interactor);
        }
        
        public void TryToStartDialogs(PlayerController interactor)
        {
            if (!_playerInRange)
                return;
            
            var interactorName = interactor.GetActiveCharacterName();
            
            dialogController.SetTalkingAudio(talkingAudio, randomizeTalkingPitch);
            //Si se trata de un dialogo de intervalos, solo se muestra el first dialog, no se usa defaultDialogue
            if (_autoNextDialog)
            {
                dialogController.SetDialogue(dialogs.firstDialog, textTypingDelay, false);
                
                //StartCoroutine(StartIntervalDialogue());
                return;
            }

            //TODO: the dialog controller will control the camera zoom when needed.
            //cam.m_Lens.OrthographicSize -= zoomAmountOnDialog; 
         
            //Si no hay frases de default, se pone el dialogo normal
            if (_isFirstInteraction && dialogs.firstDialog.sentences.Length > 0)
            {
                dialogController.SetDialogue(dialogs.firstDialog, textTypingDelay, true);
                
                _isFirstInteraction = false;
            }
            //Si no es la primera interacciôn y hay frases de default, se muestra solo el defaultDialogue
            else if (dialogs.defaultDialog.sentences.Length > 0) 
            {
                dialogController.SetDialogue(dialogs.defaultDialog, textTypingDelay, true);
            }
            
            //_isDialogueTriggered = true;

            dialogController.StartDialogs();

        }
        
        public void SetDialogs(DialogModifier dialogsModifier)
        {
            dialogs.firstDialog = dialogsModifier.dialogsSet.firstDialog;
            dialogs.defaultDialog = dialogsModifier.dialogsSet.defaultDialog;
            dialogs.wrongCharacterDialog = dialogsModifier.dialogsSet.wrongCharacterDialog;

            _autoNextDialog = dialogsModifier.autoNextDialog;
            _timeToWaitAutoNextDialog = dialogsModifier.timeToWaitAutoNextDialog;
        }
        
        IEnumerator StartIntervalDialogue()
        {
            while (true)
            {
                //dialogController.StartDialogs();
                yield return new WaitForSecondsRealtime(_timeToWaitAutoNextDialog);

                if (dialogController.IsEnded)
                {
                    yield break;
                }
            }
        }

        private void CheckForOptionalComponents()
        {
            if (interactSprite is null)
            {
                string message = string.Format(ConsoleMessages.OptionalComponentNotFound,
                    typeof(SpriteRenderer), gameObject.name);
                
                Debug.LogWarning(message);
            }
            
            if (_animator is null)
            {
                string message = string.Format(ConsoleMessages.OptionalComponentNotFound,
                    typeof(Animator), gameObject.name);
             
                Debug.LogWarning(message);
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SetIsActiveInteractSprite(true);
                
                _playerInRange = true;
                
                onAbleToInteract?.Invoke();
            }
        }
        
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                SetIsActiveInteractSprite(false);
                
                _playerInRange = false;
                
                onUnableToInteract?.Invoke();
            }
        }

        private void SetIsActiveInteractSprite(bool isActive)
        {
            if(interactSprite != null)
                interactSprite.enabled = isActive;
            if(_animator != null)
                _animator.enabled = isActive;
        }
        
    }
}
