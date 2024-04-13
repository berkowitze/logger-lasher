using UnityEditor;
using UnityEngine;

namespace EtienneEditor.Animator2D
{
    using Animator2D = Etienne.Animator2D.Animator2D;
    [CustomEditor(typeof(Animator2D))]
    public class Animator2DEditor : Editor<Animator2D>
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        public override void OnInspectorGUI()
        {
            sb.Clear();
            sb.Append("State: ");
            sb.Append(Application.isPlaying ? Target.CurrentAnimationState.Name : "Game Not Started");
            var label = new GUIContent(sb.ToString(), "State of controller");
            Rect labelRect = new Rect(190, -20, 150, EditorGUIUtility.singleLineHeight);
            GUI.Label(labelRect, label, EditorStyles.miniLabel);
            base.OnInspectorGUI();
        }

        [MenuItem("GameObject/2D Object/Sprites/Animated Sprite", priority = 0)]
        static void Create2DAnimator()
        {
            var go = EditorUtility.CreateGameObjectWithHideFlags("Animated Sprite", HideFlags.None, typeof(SpriteRenderer), typeof(Animator2D));
            Selection.activeGameObject = go;
            Undo.RegisterCreatedObjectUndo(go, "Create Animated Sprite");
        }

        private void OnSceneGUI()
        {
            if (Application.isPlaying) Repaint();
        }
    }
}
