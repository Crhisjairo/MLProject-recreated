namespace _Scripts.GameManagerSystem.Models
{
    [System.Serializable]
    public class PlayerSaveData
    {
        public bool isNew = true;
        
        public string slotName = "DefaultSaveNam";
        public string zoneName = "DefaultZoneNam";

        public int coinsAmount = 0;
        
        public bool isAbleToRun = false;
        public bool isAbleToAttack = false;
        public bool isAbleToOpenInventory = false;
    }
}
