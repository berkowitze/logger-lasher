using Etienne.Animator2D;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class Timeline : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Timeline> { }

        public event Action<Sprite> OnSelectedSpriteChanged
        {
            add => previewHolder.OnSelectedSpriteChanged += value;
            remove => previewHolder.OnSelectedSpriteChanged -= value;
        }
        public float Value01 => Value * .01f;
        public float Value { get => previewHolder.value; set => previewHolder.value = value; }
        public Sprite SelectedSprite => previewHolder.SelectedSprite;
        public bool IsPlaying => isPlaying;
        public TimedAnimationEvent SelectedEvent => selectedEventBar?.userData as TimedAnimationEvent;

        private bool isDragging = false;
        private Animation2D currentAnimation;
        private Bar selectedEventBar;
        private bool isPlaying = false;
        private double time = 0;
        private double lastTimeSinceStartup = 0;
        private readonly SpritePreviewHolder previewHolder;
        private readonly Bar selection;
        private readonly Toolbar eventbar;
        private readonly List<Bar> eventBars = new List<Bar>();

        public Timeline()
        {
            Toolbar timebar = new Toolbar();
            Add(timebar);
            eventbar = new Toolbar();
            Add(eventbar);

            previewHolder = new SpritePreviewHolder();
            Add(previewHolder);

            selection = new Bar(Color.white, "Selection");
            selection.SetLengthUnit(LengthUnit.Percent);
            Add(selection);
            previewHolder.OnValueChanged += selection.SetValue;
            SetEnabled(false);
        }


        public override void HandleEvent(EventBase evt)
        {
            if (!enabledSelf) return;
            if (evt is MouseDownEvent downEvent)
            {
                if (downEvent.button == 0)
                {
                    isDragging = true;
                    float mouseX = downEvent.localMousePosition.x;
                    GetSelectedEvent(downEvent.localMousePosition);

                    if (selectedEventBar != null)
                    {
                        selectedEventBar.SetPixelValue(mouseX);
                    }
                    else
                    {
                        previewHolder.SetPixelValue(mouseX);
                    }
                }
                else if (downEvent.button == 1)
                {
                    float mouseX = downEvent.localMousePosition.x;
                    GetSelectedEvent(downEvent.mousePosition);

                    GenericMenu menu = new GenericMenu();
                    if (SelectedEvent == null)
                    {
                        menu.AddItem(new GUIContent("Add Event"), false, () =>
                        {
                            currentAnimation.AddEvent(selection.Get01ValueFromPixelValue(mouseX));
                            SetAnimation(currentAnimation, Value);
                        });
                    }
                    else
                    {
                        TimedAnimationEvent e = SelectedEvent;
                        menu.AddItem(new GUIContent("Delete Event"), false, () =>
                        {
                            currentAnimation.DeleteEvent(e);
                            SetAnimation(currentAnimation, Value);
                        });
                    }
                    menu.ShowAsContext();
                }
            }
            if (evt is MouseUpEvent upEvent)
            {
                if (upEvent.button == 0) isDragging = false;
            }
            if (evt is MouseMoveEvent moveEvent)
            {
                if (isDragging)
                {
                    if (selectedEventBar != null)
                    {
                        selectedEventBar.SetPixelValue(moveEvent.localMousePosition.x);
                    }
                    else
                    {
                        previewHolder.SetPixelValue(moveEvent.localMousePosition.x);
                    }
                }
            }
            if (evt is MouseLeaveEvent leaveEvent)
            {
                isDragging = false;
            }
        }

        private void GetSelectedEvent(Vector2 mousePosition)
        {
            foreach (Bar bar in eventBars)
            {
                Rect layout = bar.layout;
                layout.y += layout.height;
                Rect mouseRect = new Rect(mousePosition, Vector2.one * 10f)
                {
                    center = mousePosition
                };
                if (!layout.Overlaps(mouseRect))
                {
                    selectedEventBar = null;
                    continue;
                }
                selectedEventBar = bar;
                break;
            }
        }

        public void SetAnimation(Animation2D animation, float? percentValue)
        {
            SetEnabled(true);
            currentAnimation = animation;
            previewHolder.SetAnimation(animation, percentValue);
            if (percentValue.HasValue) previewHolder.value = percentValue.Value;
            selectedEventBar = null;
            eventBars.Clear();
            eventbar.Clear();
            if (animation.TimedAnimationEvents == null) return;
            foreach (TimedAnimationEvent timedEvent in animation.TimedAnimationEvents)
            {
                Bar bar = new Bar(Color.yellow, timedEvent.EventData.Name, true);
                bar.style.width = 5;
                bar.SetLengthUnit(LengthUnit.Percent);
                bar.SetValue(timedEvent.Timing * 100);
                TextField label = new TextField("");
                label.StretchToParentSize();
                label.style.left = new StyleLength(new Length(4, LengthUnit.Pixel));
                label.style.right = new StyleLength(new Length(-100, LengthUnit.Pixel));
                label.style.unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.MiddleLeft);
                label.value = timedEvent.EventData.Name;
                label.RegisterValueChangedCallback(timedEvent.EventData.SetName);
                bar.OnValueChanged += v => timedEvent.SetTiming(v * .01f);
                bar.Add(label);
                bar.userData = timedEvent;
                eventbar.Add(bar);
                eventBars.Add(bar);
            }
        }

        public void FirstFrame()
        {
            if (!enabledSelf) return;
            previewHolder.FirstFrame();
        }
        public void LastFrame()
        {
            if (!enabledSelf) return;
            previewHolder.LastFrame();
        }
        public void PreviousFrame()
        {
            if (!enabledSelf) return;
            previewHolder.PreviousFrame();
        }
        public void NextFrame()
        {
            if (!enabledSelf) return;
            previewHolder.NextFrame();
        }
        public void TogglePlay()
        {
            if (!enabledSelf) return;
            isPlaying = !isPlaying;
            if (isPlaying) Play();
            else Pause();
        }

        private void Pause()
        {
            if (!enabledSelf) return;
            isPlaying = false;
            EditorApplication.update -= Update;
        }

        private void Play()
        {
            if (!enabledSelf) return;
            isPlaying = true;
            EditorApplication.update += Update;
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
            time = Value01 * currentAnimation.Duration;
        }

        private void Update()
        {
            if (!enabledSelf) return;
            time += EditorApplication.timeSinceStartup - lastTimeSinceStartup;
            float duration = currentAnimation.Duration;
            time %= duration;
            float value01 = (float)time / duration;
            previewHolder.SetPercentValue(value01 * 100);
            lastTimeSinceStartup = EditorApplication.timeSinceStartup;
        }
    }

}
