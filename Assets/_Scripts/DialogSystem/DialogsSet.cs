using System;

namespace _Scripts.DialogSystem
{
    /// <summary>
    /// Represents dialogs for an NPC or Player.
    /// </summary>
    [Serializable]
    public class DialogsSet
    {
        public Dialog firstDialog;
        public Dialog defaultDialog;
        public Dialog wrongCharacterDialog;
    }
}
