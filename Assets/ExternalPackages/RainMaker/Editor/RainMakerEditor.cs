﻿//
// Rain Maker (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
//

#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using System;
using ExternalPackages.RainMaker.Prefab;
using UnityEditor;
using UnityEngine;

namespace ExternalPackages.RainMaker.Editor
{
    public class RainMakerEditor : UnityEditor.Editor
    {
        private Texture2D logo;

        public override void OnInspectorGUI()
        {
            if (logo == null)
            {
                string[] guids = AssetDatabase.FindAssets("RainMakerLogo");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    logo = AssetDatabase.LoadMainAssetAtPath(path) as Texture2D;
                    if (logo != null)
                    {
                        break;
                    }
                }
            }
            if (logo != null)
            {
                const float maxLogoWidth = 430.0f;
                EditorGUILayout.Separator();
                float w = EditorGUIUtility.currentViewWidth;
                Rect r = new Rect();
                r.width = Math.Min(w - 40.0f, maxLogoWidth);
                r.height = r.width / 2.7f;
                Rect r2 = GUILayoutUtility.GetRect(r.width, r.height);
                r.x = ((EditorGUIUtility.currentViewWidth - r.width) * 0.5f) - 4.0f;
                r.y = r2.y;
                GUI.DrawTexture(r, logo, ScaleMode.StretchToFill);
                if (GUI.Button(r, "", new GUIStyle()))
                {
                    Application.OpenURL("https://u3d.as/s0Z?aid=1011lGnL");
                }
                EditorGUILayout.Separator();
            }

            DrawDefaultInspector();
        }
    }

    public class RainMakerEditor3D : RainMakerEditor
    {
    }

    [CustomEditor(typeof(RainScript2D))]
    public class RainMakerEditor2D : RainMakerEditor
    {
    }
}

#endif