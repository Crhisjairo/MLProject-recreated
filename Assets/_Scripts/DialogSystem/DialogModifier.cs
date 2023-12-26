using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.DialogSystem
{
    public class DialogModifier : MonoBehaviour
    {
        [FormerlySerializedAs("dialogsWrapper")]
        public DialogsSet dialogsSet;
        
        public bool autoNextDialog;
        public float timeToWaitAutoNextDialog = 1f;
    }
}
