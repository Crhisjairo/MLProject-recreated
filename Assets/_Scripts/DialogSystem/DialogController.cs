using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.SoundsManagers;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace _Scripts.DialogSystem
{
    [RequireComponent(typeof(SoundEmitter))]
    public class DialogController : MonoBehaviour
    {
        [SerializeField] private Canvas dialogCanvas;
        [SerializeField] private TextMeshProUGUI dialogTitle;
        [SerializeField] private TextMeshProUGUI dialog;
        [SerializeField] private Image dialogImage;
        
        private SoundEmitter _soundEmitter;

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
            _soundEmitter = GetComponent<SoundEmitter>();
        }
        
        public void SetDialogue(Dialogs dialogueSentences, float typingSpeed) 
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

        public void SetTalkingAudio(Sound soundFx)
        {
            _soundEmitter.SetFirstSound(soundFx);
        }

        public void SetRandomizePitch(bool isRandomized)
        {
            RandomizeCharacterPitch = isRandomized;
        }
        
        public void DisplayNextSentence()
        {
            if (IsTyping)
            {
                StopCoroutine(_typingCoroutine);
                dialog.text = currentSentence;
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
            
            while (true)
            {
                for (int i = 0; i <= sentence.Length; i++)
                {
                    dialog.text = sentence.Substring(0, i);

                    if (RandomizeCharacterPitch)
                    {
                        _soundEmitter.RandomizePitchFirstAudio();
                    }
                    
                    _soundEmitter.PlayFirstOneShot();
                    
                    if (i == sentence.Length)
                    {
                        IsTyping = false;
                        yield break;
                    }

                    yield return new WaitForSecondsRealtime(_typingSpeed);
                }    
            }
        }

        void EndDialogue()
        {
            //Debug.Log("Dialog finished|||");
            
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
