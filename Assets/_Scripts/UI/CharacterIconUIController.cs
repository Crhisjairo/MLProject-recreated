using _Scripts.Characters;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class CharacterIconUIController : MonoBehaviour
    {
        [SerializeField] private Image image;

        public void UpdateCharacterIconUI(Character activeCharacter)
        {
            image.sprite = activeCharacter.iconSprite;
        }
    }
}