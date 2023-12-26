using _Scripts.Controllers.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class CharacterIconUIController : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Vector3 cameraShake = new Vector3(0, -10, 0);

        [SerializeField] private float animationTime = 0.05f;

        private Vector3 _initialPos, _finalPos, _finalPosInversed;

        private void Start()
        {
            
            _initialPos = image.gameObject.transform.position;
            _finalPos = _initialPos + cameraShake;
            _finalPosInversed = _initialPos - cameraShake;
        }

        public void UpdateCharacterIconUI(Character activeCharacter)
        {
            image.sprite = activeCharacter.iconSprite;

        }

        //TODO: may produce a bug on UI when called for the first time the game loads (in editor).
        public void PlayLifeTakeAnimation()
        {
            LeanTween.move(image.gameObject, _finalPosInversed, animationTime)
                .setOnComplete(ResetIconProsition);
        }
        
        public void PlayDamageAnimation()
        {
            LeanTween.move(image.gameObject, _finalPos, animationTime)
                .setOnComplete(ResetIconProsition);
        }

        private void ResetIconProsition()
        {
            LeanTween.move(image.gameObject, _initialPos, animationTime);
        }
        
    }
}