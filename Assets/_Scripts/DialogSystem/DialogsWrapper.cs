using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.DialogSystem
{
    /// <summary>
    /// Represents dialogs for an NPC or Player.
    /// </summary>
    [System.Serializable]
    public class DialogsWrapper
    {
        public Dialog firstDialog;
        public Dialog defaultDialog;
        public Dialog wrongCharacterDialog;
    }
}
