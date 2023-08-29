using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace _Scripts.DialogSystem
{
    public class DialogController : MonoBehaviour
    {
        [SerializeField] private Canvas dialogCanvas;
        [SerializeField] private TextMeshProUGUI dialogTitle;
        [SerializeField] private TextMeshProUGUI dialog;
        [SerializeField] private Image dialogImage;

        public Sprite defaultSprite;
        public bool IsEnded { private set; get; }

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
        }
        
        public void SetDialogue(Dialogs dialogueSentences, float typingSpeed) 
        {
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

            //Impedimos que el personaje se mueva
            //GameManager.Instance.CanPlayerMove = false;
            //GameManager.Instance.CanPlayerOpenInventory = false;
            
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

                    if (i == sentence.Length)
                    {
                        IsTyping = false;
                        yield break;
                    }

                    yield return new WaitForSeconds(_typingSpeed);
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
            
            //Permitimos mover al personaje
            //GameManager.Instance.CanPlayerMove = true;
            //GameManager.Instance.CanPlayerOpenInventory = true;
        }
    }
}
