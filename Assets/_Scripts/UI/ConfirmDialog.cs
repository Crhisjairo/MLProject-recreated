using _Scripts.GameManagerSystem.SlotsManager;
using TMPro;
using UnityEngine;
namespace _Scripts.UI
{
    public class ConfirmDialog : MonoBehaviour
    {
        public string title = "default title {0}.";
        
        [SerializeField] private TextMeshProUGUI textMesh;
        
        private const string ColorPrefix = "<color=purple>";
        private const string ColorSufix = "</color>";

        
        public void SetStringParameters(string param1)
        {
            var param = string.Concat(ColorPrefix, param1, ColorSufix);
            
            textMesh.text = string.Format(title, param);
        }

        public void SetSlotParameters(Slot slot)
        {
            var param = string.Concat(ColorPrefix, slot.GetSlotName(), ColorSufix);
            
            
            textMesh.text = string.Format(title, param);
        }
    }
}
