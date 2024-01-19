using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Serialization;

namespace _Scripts.UI
{
    
    [ExecuteInEditMode]
    public class OffScreenUICull : MonoBehaviour
    {
        [FormerlySerializedAs("_viewportRectangle")] [SerializeField] RectTransform viewportRectangle;
        [FormerlySerializedAs("_ownRectTransform")] [SerializeField, Space(15)] RectTransform ownRectTransform;
     
        //will be disabled if our GUI goes outside of wanted region
        [FormerlySerializedAs("_localGraphicComponent")] [SerializeField] public Graphic localGraphicComponent;
        [FormerlySerializedAs("_optionalGO_to_On_Off")] [SerializeField] public GameObject[] optionalGoToOnOff;
        
        void Reset() {
            ownRectTransform = transform as RectTransform;
        }
        
        void Start() {
            if(viewportRectangle == null) {
                viewportRectangle = (GetComponentInParent(typeof(Canvas)) as Canvas).transform as RectTransform;
            }
        }
        
        void Update() {
            #if UNITY_EDITOR
            //while in editor, this will discard null "optional game objects", automatically.
            int prevLength = optionalGoToOnOff.Length;
     
            optionalGoToOnOff = optionalGoToOnOff.Where(go => go != null)
                                                         .ToArray();
     
            if(optionalGoToOnOff.Length != prevLength) {
                UnityEditor.EditorUtility.SetDirty(this);
            }
     
             if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode==false){ return; }
            #endif
         
            Cull();
        }
        
        void Cull() {
            if(viewportRectangle == null) { return ; }
     
            bool overlaps = ownRectTransform.rectTransfOverlaps_inScreenSpace(viewportRectangle);
     
            if (overlaps) {
                toggleElements_ifNeeded(true);
            } else {
                toggleElements_ifNeeded(false);
            }
        }
        
        void toggleElements_ifNeeded(bool requiredValue) {
     
            for (int i = 0; i < optionalGoToOnOff.Length; i++) {
                GameObject optionalGo = optionalGoToOnOff[i];
     
                if(optionalGo.activeSelf != requiredValue) {
                    optionalGo.SetActive(requiredValue);
                }
            }//end for
     
     
            if (localGraphicComponent != null   &&  localGraphicComponent.enabled != requiredValue) {
                localGraphicComponent.enabled = requiredValue;
            }
     
        }
    }
    
    static class Extensions {
     
        public static bool rectTransfOverlaps_inScreenSpace(this RectTransform rectTrans1, RectTransform rectTrans2) {
            Rect rect1 = rectTrans1.getScreenSpaceRect();
            Rect rect2 = rectTrans2.getScreenSpaceRect();
     
            return rect1.Overlaps(rect2);
        }
        
        //rect transform into coordinates expressed as seen on the screen (in pixels)
        //takes into account RectTrasform pivots
        // based on answer by Tobias-Pott
        // http://answers.unity3d.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
        public static Rect getScreenSpaceRect(this RectTransform transform) {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= ((1.0f - transform.pivot.y) * size.y);
            return rect;
        }
    }
}
