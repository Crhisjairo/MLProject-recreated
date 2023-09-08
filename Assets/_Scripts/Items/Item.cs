using System;
using System.Collections;
using _Scripts.Controllers;
using _Scripts.Enums;
using _Scripts.Items.Specs;
using _Scripts.SoundsManagers;
using UnityEngine;
using UnityEngine.Serialization;
namespace _Scripts.Items
{
    [RequireComponent(typeof(SoundEmitter))]
    public class Item : MonoBehaviour
    {
        [SerializeField] private ItemDefaultSpecs defaultSpecs;

        [SerializeField] private SpriteRenderer spriteRendererToDisable;
        [SerializeField] private BoxCollider2D colliderToDisable;
        
        [SerializeField] private float timeBeforeDestroy = 1f;
        
        private int worldId = 0; // TODO: used to know if item must be spawned or nor when load save data.

        private SoundEmitter _soundEmitter;

        public bool isGrabable = false;

        private void Awake()
        {
            _soundEmitter = GetComponent<SoundEmitter>();
        }

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
                    _soundEmitter.PlayOneShot(SoundsFX.Taken.ToString());

                    StartCoroutine(DestroyItemAfterTime());
                }
            }
            
        }

        private IEnumerator DestroyItemAfterTime()
        {
            colliderToDisable.enabled = false;
            spriteRendererToDisable.enabled = false;
            
            yield return new WaitForSecondsRealtime(timeBeforeDestroy);
            Destroy(gameObject);
        }
    }
}
