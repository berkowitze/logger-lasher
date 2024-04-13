using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class TimelineButton : ToolbarButton
    {
        private VisualElement backgroundImage = new VisualElement();
        private Texture2D normalTexture, activeTexture;
        private bool isActive;

        private TimelineButton()
        {
            AddToClassList("control-button");
            Add(backgroundImage);
        }
        /// <param name="imageName">Image's filename with extension</param>
        /// <param name="activeImageName">Image's active state filename with extension, <see cref="null"/> by default</param>
        public TimelineButton(string imageName, string activeImageName = null) : this()
        {
            normalTexture = AnimatorPath.LoadFromEditorResources<Texture2D>((EditorGUIUtility.isProSkin ? "d_" : "") + imageName);
            backgroundImage.style.backgroundImage = new StyleBackground(Background.FromTexture2D(normalTexture));
            if (activeImageName == null) return;
            activeTexture = AnimatorPath.LoadFromEditorResources<Texture2D>((EditorGUIUtility.isProSkin ? "d_" : "") + activeImageName);
            clicked += ToggleActiveState;
        }

        public void ToggleActiveState()
        {
            isActive = !isActive;
            Background background = Background.FromTexture2D(isActive ? activeTexture : normalTexture);
            backgroundImage.style.backgroundImage = new StyleBackground(background);
        }
    }
}
