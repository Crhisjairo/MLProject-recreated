using TMPro;
using UnityEngine;
namespace _Scripts.UI
{
    public class ConfirmDialog : MonoBehaviour
    {
        public string title = "default title {0}.";
        
        [SerializeField] private TextMeshProUGUI textMesh;

        public void SetStringParameters(string param1)
        {
            textMesh.text = string.Format(title, param1);
        }
    }
}
