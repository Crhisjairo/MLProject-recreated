using UnityEngine;

namespace _Scripts.DialogSystem
{
    [System.Serializable]
    public class Dialog
    {
        public string[] titles;

        [TextArea(3, 10)]
        public string[] sentences;

        public Sprite[] spritesForSentences;
    }
}
