namespace _Scripts.GameManagerSystem.Models
{
    [System.Serializable]
    public class PlayerSaveData
    {
        public string saveDataName = "DefaultSaveName";
        
        public bool isAbleToRun = false;
        public bool isAbleToAttack = false;
        public bool isAbleToOpenInventory = false;
    }
}
