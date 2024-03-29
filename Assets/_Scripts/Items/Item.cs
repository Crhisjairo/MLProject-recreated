using System.Collections;
using _Scripts.Controllers;
using _Scripts.Items.Specs;
using _Scripts.Shared.Enums;
using SoundsManagers._Scripts.SoundsManagers;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Items
{
    [RequireComponent(typeof(SoundFXEmitter))]
    public class Item : MonoBehaviour
    {
        [SerializeField] private ItemDefaultSpecs defaultSpecs;

        [SerializeField] private SpriteRenderer spriteRendererToDisable;
        [SerializeField] private BoxCollider2D colliderToDisable;
        
        [SerializeField] private float timeBeforeDestroy = 1f;
        
        // TODO: used to know if item must be spawned or nor when load save data.

        private SoundFXEmitter _soundFXEmitter;

        public UnityEvent onItemGrabed;

        public bool isGrabable;
        public bool useDefaultAnimation;
        public float defaultOffsetAnimation = 0.2f;
        public float defaultAnimationSpeed = 1f;

        private void Awake()
        {
            _soundFXEmitter = GetComponent<SoundFXEmitter>();
        }

        private void Start()
        {
            if (useDefaultAnimation)
            {
                var destAnimation = transform.position.y + defaultOffsetAnimation;
                
                LeanTween.moveY(gameObject, destAnimation, defaultAnimationSpeed)
                    .setEaseInQuad()
                    .setEaseOutSine()
                    .setLoopPingPong();
            }
        }

        private void GiveItemToPlayerInventory(PlayerController playerController)
        {
            if (playerController.TryToAddItemToInventory(gameObject))
            {
                onItemGrabed?.Invoke();
                
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
                    onItemGrabed?.Invoke();
                    
                    defaultSpecs.TakeEffect(playerController);
                    _soundFXEmitter.PlayOneShot(SoundsFX.Taken.ToString());

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
