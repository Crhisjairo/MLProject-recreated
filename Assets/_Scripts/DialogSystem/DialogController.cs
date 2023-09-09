using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.SoundsManagers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

        private Animator _nextDialogAnimator;
        private Image _nextDialogSpriteRenderer;
        
        private AudioSource _audioSource;

        public bool RandomizeCharacterPitch = true;
        
        public Sprite defaultSprite;
        public bool IsEnded { private set; get; }

        public UnityEvent onDialogStarted, onDialogEnded;

        Queue<string> sentencesSorted;
        Queue<Sprite> spritesSorted;

        bool IsTyping { set; get; }
        float _typingSpeed;
        string currentSentence;
        
        Coroutine _typingCoroutine;
        
        void Awake()
        {
            sentencesSorted = new Queue<string>();
            spritesSorted = new Queue<Sprite>();
            dialogCanvas.enabled = false; //Desactivamos el canvas grande
            _audioSource = GetComponent<AudioSource>();

            _nextDialogAnimator = nextDialogSpriteGameObject.GetComponent<Animator>();
            _nextDialogSpriteRenderer = nextDialogSpriteGameObject.GetComponent<Image>();
        }

        private void Start()
        {
            ActivateNextDialogSprite(false);
        }

        private void ActivateNextDialogSprite(bool isActive)
        {
            _nextDialogAnimator.enabled = isActive;
            _nextDialogSpriteRenderer.enabled = isActive;
        }

        public void SetDialogue(Dialog dialogueSentences, float typingSpeed) 
        {
            onDialogStarted?.Invoke();
            
            _typingSpeed = typingSpeed;
            
            //Damos nombre al cuadro de texto
            dialogTitle.text = dialogueSentences.title;
            IsEnded = false;
            sentencesSorted.Clear(); //limpiamos las frases que estan en el DialogueManager.
            spritesSorted.Clear();

            foreach (var sentence in dialogueSentences.sentences)
            {
                sentencesSorted.Enqueue(sentence);
            }

            if (dialogueSentences.spritesForSentences != null)
            {
                foreach (var sprite in dialogueSentences.spritesForSentences)
                {
                    spritesSorted.Enqueue(sprite);
                }

            }
        }

        public void SetTalkingAudio(Sound soundFx, bool randomizePitch = false)
        {
            _audioSource.clip = soundFx.clip;
            _audioSource.pitch = soundFx.pitch;

            RandomizeCharacterPitch = randomizePitch;
        }
        
        public void DisplayNextSentence()
        {
            if (IsTyping)
            {
                StopCoroutine(_typingCoroutine);
                dialog.text = currentSentence;
                ActivateNextDialogSprite(true);
                IsTyping = false;
                return;
            }
            
            if (sentencesSorted.Count == 0) 
            {
                EndDialogue();
                return;
            }

            currentSentence = sentencesSorted.Dequeue();
            if (spritesSorted.Count != 0)
            {
                defaultSprite = spritesSorted.Dequeue();
            }
            
            //Activamos el cuadro de dialogo
            dialogCanvas.enabled = true;
            
            //Debug.Log(sentence.Length);

            _typingCoroutine = StartCoroutine(TypingEffect(currentSentence));
        }

        IEnumerator TypingEffect(string sentence)
        {
            IsTyping = true;
            dialogImage.sprite = defaultSprite;
            ActivateNextDialogSprite(false);
            
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
                        ActivateNextDialogSprite(true);
                        
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
            
            if (RandomizeCharacterPitch)
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
            ActivateNextDialogSprite(false);
            
            //limpiamos lastSentence
            currentSentence = String.Empty;
            
            //ocultamos el cuadro de dialogo
            dialogCanvas.enabled = false;
            IsEnded = true;
            
            onDialogEnded?.Invoke();
            
            //Permitimos mover al personaje
            //GameManager.Instance.CanPlayerMove = true;
            //GameManager.Instance.CanPlayerOpenInventory = true;
        }
    }
}
