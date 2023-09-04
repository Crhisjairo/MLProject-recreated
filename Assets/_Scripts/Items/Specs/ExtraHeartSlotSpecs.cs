using _Scripts.Controllers;
using UnityEngine;
namespace _Scripts.Items.Specs
{
    [CreateAssetMenu(fileName = "Extra Heart Slot", menuName = "ItemsSpecs/ExtraHeartSlot", order = 3)]
    public class ExtraHeartSlotSpecs : ItemDefaultSpecs
    {
        public override void TakeEffect(PlayerController playerController)
        {
            Debug.Log("Item " + itemName + " usado por: " + playerController.GetActiveCharacterName());
            
            playerController.TakeExtraHeartSlot();
        }
    }
}
