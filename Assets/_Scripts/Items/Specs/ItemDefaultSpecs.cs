using _Scripts.Controllers;
using UnityEngine;

namespace _Scripts.Items.Specs
{
    public abstract class ItemDefaultSpecs : ScriptableObject
    {
        //TODO: change that by a unique ID for each Item when SaveData Manager is created.
        public int id;
        
        public string itemName;

        public abstract void TakeEffect(PlayerController playerController);
    }
}
