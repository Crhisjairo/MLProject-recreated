using UnityEngine;
namespace _Scripts.DialogSystem
{
    public class DialogModifier : MonoBehaviour
    {
        public DialogsWrapper dialogsWrapper;
        
        public bool autoNextDialog;
        public float timeToWaitAutoNextDialog = 1f;
    }
}
