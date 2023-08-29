using UnityEngine;

namespace _Scripts.DialogSystem
{
    [System.Serializable]
    public class Dialogs
    {
        public string title;
        
        [TextArea(3, 10)] 
        public string[] sentences;
        
        public Sprite[] spritesForSentences;
    }
}
