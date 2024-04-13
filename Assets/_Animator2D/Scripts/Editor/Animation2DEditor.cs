using Etienne.Animator2D;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EtienneEditor.Animator2D
{
    [CustomEditor(typeof(Animation2D)), CanEditMultipleObjects]
    public class Animation2DEditor : Editor<Animation2D>
    {
        private int index = 0;
        private bool isEnabled = false;
        private Animation2D currentTarget;
        private double lastTimeSinceStartup;
        private float currentFrameDuration;

        [MenuItem("Assets/Create/Etienne/2D/Animator/Animation")]
        private static void CreateAnimation2DEtienne() => CreateAnimation2D();

        [MenuItem("Assets/Create/2D/Animator/Animation")]
        private static void CreateAnimation2D()
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (Object item in Selection.objects)
            {
                if (item is not Sprite sprite) continue;
                sprites.Add(sprite);
            }
            Animation2D animation = CreateInstance<Animation2D>();
            animation.SetSprites(sprites.ToArray());
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            path = ProjectWindowUtil.IsFolder(Selection.activeInstanceID) ? path : Path.GetDirectoryName(path);
            ProjectWindowUtil.CreateAsset(animation, $"{path}/New Animation2D.asset");
        }

        public override bool HasPreviewGUI()
        {
            return currentTarget.Frames != null && currentTarget.Frames.Length >= 1;
        }

        private void OnEnable()
        {
            currentTarget = targets[0] as Animation2D;
            if (!HasPreviewGUI()) return;
            index = 0;
            isEnabled = true;
            EditorApplication.update += Update;
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
            currentFrameDuration = currentTarget.Frames[index].Duration * (1.0f / currentTarget.FPS);
        }

        private void OnDisable()
        {
            index = 0;
            isEnabled = false;
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (!isEnabled || currentTarget.Frames.Length <= 0) return;
            double time = EditorApplication.timeSinceStartup - lastTimeSinceStartup;
            if (time >= currentFrameDuration)
            {
                index++;
                index %= currentTarget.Frames.Length;
                Repaint();
                currentFrameDuration = currentTarget.Frames[index].Duration * (1.0f / currentTarget.FPS);
                lastTimeSinceStartup = EditorApplication.timeSinceStartup;
            }
        }

        public override void DrawPreview(Rect previewArea)
        {
            if (!isEnabled)
            {
                isEnabled = true;
                Update();
            }
            Sprite sprite = currentTarget.Frames[index % currentTarget.Frames.Length].Sprite;
            if (sprite == null) return;
            DrawSpritePreview(previewArea, sprite);

            Rect labelPosition = previewArea;
            const int height = 35;
            labelPosition.y = previewArea.height - height;
            labelPosition.height = height;
            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.UpperCenter
            };
            string text = $"{currentTarget.name}{System.Environment.NewLine}{index}/{currentTarget.Frames.Length}";
            GUI.Label(labelPosition, text, style);
        }

        public static void DrawSpritePreview(Rect position, Sprite sprite, bool drawBackground = true)
        {
            Rect spriteRect = sprite.rect;
            Vector2 textureSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Rect coords = spriteRect;
            coords.height = spriteRect.height / textureSize.y;
            coords.width = spriteRect.width / textureSize.x;
            coords.x = spriteRect.x / textureSize.x;
            coords.y = spriteRect.y / textureSize.y;

            Vector2 ratio = new(spriteRect.width / spriteRect.height, spriteRect.height / spriteRect.width);

            Vector2 center = position.center;
            if (position.width * ratio.y <= position.height) position.height = position.width * ratio.y;
            else position.width = position.height * ratio.x;
            position.center = center;

            if (drawBackground) EditorGUI.DrawRect(position, new Color(1, 1, 1, .1f));
            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }

    }
}
