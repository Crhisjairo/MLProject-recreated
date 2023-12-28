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
            CalculatePositions();
        }

        public void CalculatePositions()
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
            if (!LeanTween.isTweening(image.gameObject))
            {
                LeanTween.move(image.gameObject, _finalPosInversed, animationTime).setLoopPingPong(1);
            }
        }
        
        public void PlayDamageAnimation()
        {
            if (!LeanTween.isTweening(image.gameObject))
            {
                LeanTween.move(image.gameObject, _finalPos, animationTime).setLoopPingPong(1);
            }
        }
        
    }
}