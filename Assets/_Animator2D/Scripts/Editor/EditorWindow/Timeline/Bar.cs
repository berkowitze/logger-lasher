using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class Bar : VisualElement, INotifyValueChanged<float>
    {
        public Action<float> OnValueChanged;
        public float value
        {
            get => m_value; set
            {
                SetValueWithoutNotify(value);
                OnValueChanged?.Invoke(value);
            }
        }
        private float m_value;
        private LengthUnit lengthUnit = LengthUnit.Pixel;

        public Bar(Color color, string name = "", bool isPickable = false)
        {
            this.name = name;
            focusable = false;
            style.position = Position.Absolute;
            style.backgroundColor = color;
            this.StretchToParentSize();
            style.width = 1;
            pickingMode = isPickable ? PickingMode.Position : PickingMode.Ignore;
        }

        public void SetLengthUnit(LengthUnit unit)
        {
            lengthUnit = unit;
        }

        public void SetVisibility(bool visible) => this.visible = visible;

        public void Show() => SetVisibility(true);

        public void Hide() => SetVisibility(false);

        public void SetValue(float newValue)
        {
            value = newValue;
        }

        public void SetValueWithoutNotify(float newValue)
        {
            style.left = new StyleLength(new Length(newValue /*- layout.width*/, lengthUnit));
            m_value = newValue;
        }

        public void SetPixelValue(float pixelValue)
        {
            value = Get01ValueFromPixelValue(pixelValue) * 100;
        }

        public float Get01ValueFromPixelValue(float pixelValue)
        {
            return new Etienne.Range(0f, parent.layout.width).Normalize(pixelValue);
        }
    }
}