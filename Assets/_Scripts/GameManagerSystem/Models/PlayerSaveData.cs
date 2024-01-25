using _Scripts.Shared.Enums;
using UnityEngine.Serialization;

namespace _Scripts.GameManagerSystem.Models
{
    //TODO: Properties can be separate into many classes.
    [System.Serializable]
    public class PlayerSaveData
    {
        // Save data slot information
        public bool isNew = true;
        public string slotName = "Empty slot";
        
        public ScenesNames lastSceneName = ScenesNames.Tutorial;
        public string zoneName = "Rebirth Cave";

        public bool isAutoSaved;
        public PlayerPosition lastPlayerPosition;
        
        // Player inventory and status
        [FormerlySerializedAs("CharacterSaveData")] public CharacterSaveData[] characterSaveData;
        
        public int coinsAmount;
        
        // Player abilities
        public bool isAbleToRun;
        public bool isAbleToAttack;
        public bool isAbleToOpenInventory;

        // TODO: Enemies defeated
        
        // TODO: Items got
        
    }
}
