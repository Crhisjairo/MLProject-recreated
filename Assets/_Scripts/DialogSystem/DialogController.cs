using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.Controllers;
using _Scripts.Shared.Enums;
using SoundsManagers._Scripts.SoundsManagers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Scripts.DialogSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class DialogController : MonoBehaviour
    {
        [SerializeField] private Canvas dialogCanvas;
        [SerializeField] private TextMeshProUGUI dialogTitle;
        [SerializeField] private TextMeshProUGUI dialog;
        [SerializeField] private Image dialogImage;
        [SerializeField] private GameObject nextDialogSpriteGameObject;

        [SerializeField] private PlayerController playerController;
        
        private Animator _nextDialogAnimator;
        private Image _nextDialogSpriteRenderer;
        
        private AudioSource _audioSource;

        [FormerlySerializedAs("RandomizeCharacterPitch")] public bool randomizeCharacterPitch = true;
        
        public Sprite defaultSprite;
        public bool IsEnded { private set; get; }

        public UnityEvent onDialogStarted, onDialogEnded;

        Queue<string> _titlesSorted;
        Queue<string> _sentencesSorted;
        Queue<Sprite> _spritesSorted;

        bool IsTyping { set; get; }
        float _typingSpeed;
        private string _currentTitle;
        string _currentSentence;
        
        Coroutine _typingCoroutine;

        private PlayerActionMaps _lastActionMap;
        
        void Awake()
        {
            _titlesSorted = new Queue<string>();
            _sentencesSorted = new Queue<string>();
            _spritesSorted = new Queue<Sprite>();
            
            
            dialogCanvas.enabled = false; //Desactivamos el canvas grande
            _audioSource = GetComponent<AudioSource>();

            _nextDialogAnimator = nextDialogSpriteGameObject.GetComponent<Animator>();
            _nextDialogSpriteRenderer = nextDialogSpriteGameObject.GetComponent<Image>();
        }

        private void Start()
        {
            IsActiveNextDialogSprite(false);
        }

        private void IsActiveNextDialogSprite(bool isActive)
        {
            _nextDialogAnimator.enabled = isActive;
            _nextDialogSpriteRenderer.enabled = isActive;
        }

        public void SetDialogue(Dialog dialogueSentences, float typingSpeed, bool autoDialog) 
        {
            onDialogStarted?.Invoke();
            
            _typingSpeed = typingSpeed;
            IsEnded = false;

            _lastActionMap = playerController.GetCurrentActionMap();
            playerController.ChangeActionMapTo(PlayerActionMaps.InDialog);
            
            IsActiveNextDialogSprite(false);
            
            // limpiamos las frases que estan en el DialogController.
            _titlesSorted.Clear();
            _sentencesSorted.Clear(); 
            _spritesSorted.Clear();

            if (dialogueSentences.titles != null)
            {
                foreach (var title in dialogueSentences.titles)
                    _titlesSorted.Enqueue(title);
            }
            
            foreach (var sentence in dialogueSentences.sentences)
            {
                _sentencesSorted.Enqueue(sentence);
            }

            if (dialogueSentences.spritesForSentences != null)
            {
                foreach (var sprite in dialogueSentences.spritesForSentences)
                {
                    _spritesSorted.Enqueue(sprite);
                }
            }
        }

        public void SetTalkingAudio(Sound soundFx, bool randomizePitch = false)
        {
            _audioSource.clip = soundFx.clip;
            _audioSource.pitch = soundFx.pitch;

            randomizeCharacterPitch = randomizePitch;
        }

        public void StartDialogs()
        {
            DisplayNextSentence();
        }
        
        public void DisplayNextSentenceInput(InputAction.CallbackContext inputContext)
        {
            if (!inputContext.performed)
                return;
            
            DisplayNextSentence();
        }
        
        private void DisplayNextSentence()
        {
            if (IsTyping)
            {
                StopCoroutine(_typingCoroutine);
                dialog.text = _currentSentence;
                IsActiveNextDialogSprite(true);
                IsTyping = false;

                return;
            }
            
            if (_sentencesSorted.Count == 0) 
            {
                EndDialogue();
                return;
            }

            if (_titlesSorted.Count != 0)
            {
                _currentTitle = _titlesSorted.Dequeue();
            }
            
            _currentSentence = _sentencesSorted.Dequeue();
            
            if (_spritesSorted.Count != 0)
            {
                defaultSprite = _spritesSorted.Dequeue();
            }
            
            //Activamos el cuadro de dialogo
            dialogCanvas.enabled = true;
            
            //Debug.Log(sentence.Length);

            _typingCoroutine = StartCoroutine(TypingEffect(_currentSentence));
        }

        IEnumerator TypingEffect(string sentence)
        {
            IsTyping = true;
            dialogTitle.text = _currentTitle;
            dialogImage.sprite = defaultSprite;
            IsActiveNextDialogSprite(false);
            
            while (true)
            {
                for (int i = 0; i <= sentence.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        PlayTalkingAudio();
                    }
                    
                    dialog.text = sentence.Substring(0, i);

                    if (i == sentence.Length)
                    {
                        IsTyping = false;
                        IsActiveNextDialogSprite(true);
                        
                        yield break;
                    }

                    yield return new WaitForSecondsRealtime(_typingSpeed);
                }    
            }
        }

        private void PlayTalkingAudio()
        {
            if(_audioSource.clip is null)
                return;
            
            if (randomizeCharacterPitch)
                RandomizePitchTalkingAudio();
                    
            _audioSource.PlayOneShot(_audioSource.clip);
        }

        private void RandomizePitchTalkingAudio()
        {
            var pitch = _audioSource.pitch;
            
            _audioSource.pitch = Random.Range(pitch - 0.01f, pitch + 0.01f);
        }

        void EndDialogue()
        {
            IsActiveNextDialogSprite(false);
            
            //limpiamos lastSentence
            _currentSentence = String.Empty;
            
            //ocultamos el cuadro de dialogo
            dialogCanvas.enabled = false;
            IsEnded = true;
            
            onDialogEnded?.Invoke();
            playerController.ChangeActionMapTo(_lastActionMap);
            
            //Permitimos mover al personaje
            //GameManager.Instance.CanPlayerMove = true;
            //GameManager.Instance.CanPlayerOpenInventory = true;
        }
    }
}
