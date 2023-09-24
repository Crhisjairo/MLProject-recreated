using UnityEngine.Serialization;
namespace _Scripts.Models
{
    [System.Serializable]
    public class PlayerModel
    {
        public bool isAbleToRun = false;
        public bool isAbleToAttack = false;
        public bool isAbleToOpenInventory = false;
    }
}
