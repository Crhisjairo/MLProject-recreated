using System;
using _Scripts.Controllers;
using _Scripts.Enums;
using _Scripts.Items.Specs;
using UnityEngine;
namespace _Scripts.Items
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private ItemDefaultSpecs defaultSpecs;
        private int worldId = 0; // TODO: used to know if item must be spawned or nor when load save data.

        public bool isGrabable = false;

        private void GiveItemToPlayerInventory(PlayerController playerController)
        {
            if (playerController.TryToAddItemToInventory(gameObject))
            {
                Debug.Log("Item added to player: " + playerController.GetActiveCharacterName());
                Debug.Log("NOT IMPLEMENTED YET!");
                //TODO: Look if the object exists after Destroy.
                
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag(Tags.Player.ToString()))
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

                if (isGrabable)
                    GiveItemToPlayerInventory(playerController);
                else
                {
                    defaultSpecs.TakeEffect(playerController);
                 
                    Destroy(gameObject);
                }
            }
            
        }
    }
}
