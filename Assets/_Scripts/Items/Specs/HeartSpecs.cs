using _Scripts.Controllers;
using UnityEngine;

namespace _Scripts.Items.Specs
{
    [CreateAssetMenu(fileName = "Heart", menuName = "ItemsSpecs/Heart", order = 2)]
    public class HeartSpecs: ItemDefaultSpecs
    {
        public int lifeToGive = 1;
        
        public override void TakeEffect(PlayerController playerController)
        {
            Debug.Log("Item " + itemName + " usado por: " + playerController.GetActiveCharacterName());

            playerController.TakeLife(lifeToGive);
        }
    }
}
