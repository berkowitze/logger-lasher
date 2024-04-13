using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class SpritePreview : VisualElement
    {
        public SpritePreview()
        {
            style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
            style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            style.borderRightColor = Color.gray;
            style.borderRightWidth = 1;
            style.borderLeftColor = Color.gray;
            style.borderLeftWidth = 1;
            style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }

        public void SetSprite(Sprite sprite)
        {
            style.backgroundImage = new StyleBackground(sprite);
        }

        public void SetSize(int percent)
        {
            style.width = new StyleLength(new Length(percent, LengthUnit.Percent));
        }

        public void SetVisibility(bool visible) => this.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        public void Show() => SetVisibility(true);

        public void Hide() => SetVisibility(false);

        public void SetIsSelected(bool isSelected) => style.backgroundColor = isSelected ? Color.gray : Color.clear;
        public void Select() => SetIsSelected(true);
        public void UnSelect() => SetIsSelected(false);
    }
}