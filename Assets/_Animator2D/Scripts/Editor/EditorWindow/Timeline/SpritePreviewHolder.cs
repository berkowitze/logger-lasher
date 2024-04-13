using Etienne.Animator2D;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class SpritePreviewHolder : VisualElement, INotifyValueChanged<float>
    {
        public Sprite SelectedSprite => previews[selectedIndex].style.backgroundImage.value.sprite;

        public event Action<Sprite> OnSelectedSpriteChanged;
        public event Action<float> OnValueChanged;

        /// <summary>
        /// Value In Percentage, use <see cref="SetPixelValue"/> to set with pixel values
        /// </summary>
        public float value { get => m_value; set => SetPercentValue(value); }
        private float m_value;

        private Etienne.Range range = new Etienne.Range(0);
        private Queue<SpritePreview> previewPool = new Queue<SpritePreview>();
        private List<SpritePreview> previews = new List<SpritePreview>();

        private int selectedIndex;
        private bool hasSelection = false;

        public SpritePreviewHolder()
        {
            style.flexDirection = FlexDirection.Row;
            style.height = new StyleLength(new Length(100f, LengthUnit.Percent));
        }

        private void UpdateSelection()
        {
            int newSelectedIndex = 0;
            float pixelValue = range.Lerp(value * .01f);
            for (int i = 0; i < previews.Count; i++)
            {
                SpritePreview preview = previews[i];
                if (pixelValue < preview.layout.xMin || pixelValue > preview.layout.xMax) continue;
                newSelectedIndex = i;
                break;
            }
            if (previews.Count <= 0) return;
            if (hasSelection && newSelectedIndex == selectedIndex) return;
            if (hasSelection) previews[selectedIndex].UnSelect();
            selectedIndex = newSelectedIndex;
            previews[selectedIndex].Select();
            OnSelectedSpriteChanged?.Invoke(SelectedSprite);
            hasSelection = true;
        }

        public void SetAnimation(Animation2D animation, float? percentValue)
        {
            if (percentValue.HasValue) SetValueWithoutNotify(percentValue.Value);
            SetEnabled(true);
            if (hasSelection) previews[selectedIndex]?.UnSelect();
            hasSelection = false;
            int spriteCount = animation.Frames.Length;
            for (int i = previewPool.Count; i < spriteCount; i++) previewPool.Enqueue(new SpritePreview());
            for (int i = 0; i < previews.Count; i++)
            {
                previews[i].Hide();
                previewPool.Enqueue(previews[i]);
            }
            previews.Clear();
            for (int i = 0; i < spriteCount; i++)
            {
                SpritePreview preview = previewPool.Dequeue();
                Frame frame = animation.Frames[i];
                preview.SetSprite(frame.Sprite);
                preview.SetSize(Mathf.RoundToInt(frame.Duration * 100));
                preview.Show();
                previews.Add(preview);
                Insert(i, preview);
            }
            UpdateSelection();
        }

        public void FirstFrame() => SetPixelValue(previews[0].layout.xMin + 2);

        public void LastFrame() => SetPixelValue(previews[^1].layout.xMin + 2);

        public void PreviousFrame()
        {
            int value = selectedIndex - 1;
            if (value < 0) value = previews.Count - 1;
            SetPixelValue(previews[value].layout.xMin + 2);
        }

        public void NextFrame() => SetPixelValue(previews[(selectedIndex + 1) % previews.Count].layout.xMin + 2);

        public void SetPixelValue(float pixelValue)
        {
            range.Min = 0;
            range.Max = layout.width;
            SetPercentValue(range.Normalize(pixelValue) * 100);
        }

        public void SetPercentValue(float percentValue)
        {
            OnValueChanged?.Invoke(percentValue);
            SetValueWithoutNotify(percentValue);
        }

        public void SetValueWithoutNotify(float newValue)
        {
            m_value = newValue;
            UpdateSelection();
        }
    }
}
