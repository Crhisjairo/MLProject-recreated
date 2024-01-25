using _Scripts.Controllers;
using UnityEngine;
namespace _Scripts.Items.Specs
{
    [CreateAssetMenu(fileName = "TwilightCutieMark", menuName = "ItemsSpecs/TwilightCutieMark", order = 5)]
    public class TwilightCutieMark : ItemDefaultSpecs
    {

        public override void TakeEffect(PlayerController playerController)
        {
            playerController.SetIsAbleToAttack(true);
        }
    }
}
