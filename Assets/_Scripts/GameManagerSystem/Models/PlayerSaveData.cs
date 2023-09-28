using _Scripts.Enums;
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

        public bool isAutoSaved = false;
        public PlayerPosition lastPlayerPosition;
        
        // Player inventory and status
        public CharacterSaveData[] CharacterSaveData;
        
        public int coinsAmount = 0;
        
        // Player abilities
        public bool isAbleToRun = false;
        public bool isAbleToAttack = false;
        public bool isAbleToOpenInventory = false;

        // TODO: Enemies defeated
        
        // TODO: Items got
        
    }
}
