using _Scripts.Controllers;
using UnityEngine;
namespace _Scripts.Items.Specs
{
    [CreateAssetMenu(fileName = "Key", menuName = "ItemsSpecs/Key", order = 4)]
    public class KeySpecs: ItemDefaultSpecs
    {
        public DoorKeyType type = DoorKeyType.A;

        public override void TakeEffect(PlayerController playerController)
        {
            // TODO: The door will check if the key is correct or not. After that, the key must be deleted.
        }
    }
}
