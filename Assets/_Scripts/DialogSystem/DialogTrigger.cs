using System;
using System.Collections;
using _Scripts.Controllers;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.DialogSystem
{
    /// <summary>
    /// Checks the correct dialog to set on DialogManager.
    /// </summary>
    public class DialogTrigger : MonoBehaviour
    {
        [SerializeField] DialogController dialogController;
        [SerializeField] CinemachineVirtualCamera cam;
        public float zoomAmountOnDialog = 0;
        public float speedZoom = 0.5f;
        
        public bool _startOnAwake;

        public bool _autoNextDialog;
        public float _timeToWaitAutoNextDialog = 1f;

        public bool onlyInteractWithSpecifCharacter = false;
        public string characterNameAbleToInteractWith;

        [Range(0f, 0.3f)]
        public float textTypingDelay = 0.1f;
        
        public Dialogs firstDialog;
        public Dialogs defaultDialog;
        public Dialogs wrongCharacterDialog;
        
        bool _isFirstInteraction = true;
        bool _isDialogueTriggered;
        bool _canInteract;

        float currentCamZoom = 0f;

        void Start()
        {
            currentCamZoom = cam.m_Lens.OrthographicSize;

            if (_startOnAwake)
            {
                _autoNextDialog = _startOnAwake;
                dialogController.SetDialogue(defaultDialog, textTypingDelay);
                StartCoroutine(StartIntervalDialogue());
            }
        }
        
        /// <summary>
        /// Muestra el siguiente dialogo correspondiente en función si el personaje es correcto o no.
        /// Se debería crear un método en especifico para mostrar el dialogo incorrecto. Usar este método
        /// temporalmente.
        /// </summary>
        /// <param name="interactor">Nombre del personaje con el que se interactua.</param>
        public void SetDialogByContext(PlayerController interactor)
        {
            var interactorName = interactor.GetActiveCharacterName();
            var isWrongCharacter = interactorName.Equals(characterNameAbleToInteractWith);
        
            //Si se trata de un dialogo de intervalos, solo se muestra el dialogue, no se usa defaultDialogue
            if (_autoNextDialog)
            {
                dialogController.SetDialogue(firstDialog, textTypingDelay);
                StartCoroutine(StartIntervalDialogue());
                return;
            }

            
            if (!_isDialogueTriggered)
            {
                cam.m_Lens.OrthographicSize -= zoomAmountOnDialog; 
                //Si es el mal personaje que estâ interactuando
                if (isWrongCharacter)
                {
                    dialogController.SetDialogue(wrongCharacterDialog, textTypingDelay);
                } 
                //Si no hay frases de default, se pone el dialogo normal
                else if (_isFirstInteraction && firstDialog.sentences.Length > 0)
                {
                    dialogController.SetDialogue(firstDialog, textTypingDelay);
                    _isFirstInteraction = false;
                }
                //Si no es la primera interacciôn y hay frases de default, se muestra solo el defaultDialogue
                else if (defaultDialog.sentences.Length > 0) 
                {
                    dialogController.SetDialogue(defaultDialog, textTypingDelay);
                }
                
                _isDialogueTriggered = true;
            }

            dialogController.DisplayNextSentence();

            if (dialogController.IsEnded)
            {
                cam.m_Lens.OrthographicSize += zoomAmountOnDialog;
                _isDialogueTriggered = false;
            }
        
        }
        
        IEnumerator StartIntervalDialogue()
        {
            while (true)
            {
                dialogController.DisplayNextSentence();
                yield return new WaitForSeconds(_timeToWaitAutoNextDialog);

                if (dialogController.IsEnded)
                {
                    yield break;
                }
            }
        }
        
    }
}
