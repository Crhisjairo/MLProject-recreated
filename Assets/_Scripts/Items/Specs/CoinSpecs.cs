using _Scripts.Controllers;
using UnityEngine;

namespace _Scripts.Items.Specs
{
    [CreateAssetMenu(fileName = "Coin", menuName = "ItemsSpecs/Coin", order = 1)]
    public class CoinSpecs : ItemDefaultSpecs
    {
        public int amount = 1;

        public override void TakeEffect(PlayerController playerController)
        {
            Debug.Log("Item " + itemName + " usado por: " + playerController.GetActiveCharacterName());
            
            playerController.AddCoins(amount);
            
        }
    }
}
