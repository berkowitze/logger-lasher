using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Etienne.Animator2D
{
    using AnimationState2D = AnimatorController2D.AnimationState2D;
    public class Animator2D : MonoBehaviour
    {
        public AnimationState2D NextAnimationState => nextAnimationState;
        public AnimationState2D CurrentAnimationState => currentAnimationState;
        public AnimationState2D PreviousAnimationState => previousAnimationState;

        [SerializeField] private AnimatorController2D controller;
        private AnimationState2D currentAnimationState, nextAnimationState, previousAnimationState;
        private new SpriteRenderer renderer;
        private Dictionary<string, AnimationState2D> animationStates;
        private float animationTimer;
        private List<IAnimationEvent.AnimationEventData> performedEvents = new List<IAnimationEvent.AnimationEventData>();

        private void Start()
        {
            renderer = GetComponentInChildren<SpriteRenderer>();
            animationStates = new Dictionary<string, AnimationState2D>();
            foreach (AnimationState2D animationState in controller.AnimationStates)
            {
                animationStates.Add(animationState.Name, animationState);
            }
            currentAnimationState = controller.AnimationStates[0];
        }

        private void OnEnable()
        {
            if (!Application.isPlaying || renderer == null) return;
            currentAnimationState = controller.AnimationStates[0];
        }

        private void OnDisable()
        {
            if (!Application.isPlaying) return;
        }

        private void Update()
        {
            animationTimer += Time.deltaTime;
            Animation2D animation = currentAnimationState.Animation;
            if (animation == null)
            {
                renderer.sprite = null;
                gameObject.SetActive(false);
                return;
            }
            if (!currentAnimationState.IsLooping && animationTimer >= animation.Duration)
            {
                if (currentAnimationState.OutStateName != string.Empty) SetState(currentAnimationState.OutStateName, true);
                return;
            }
            if (animationTimer >= animation.Duration)
            {
                animationTimer = 0f;
                performedEvents.Clear();
            }
            float totalDuration = 0f;
            for (int i = 0; i < animation.Frames.Length; i++)
            {
                Frame frame = animation.Frames[i];
                totalDuration += frame.Duration * animation.FrameDuration;
                if (animationTimer < totalDuration) continue;
                renderer.sprite = frame.Sprite;
            }

            IAnimationEvent[] animationEvents = GetComponents<IAnimationEvent>();
            for (int i = 0; i < animation.TimedAnimationEvents.Length; i++)
            {
                TimedAnimationEvent eventData = animation.TimedAnimationEvents[i];
                if (performedEvents.Contains(eventData.EventData)) continue;
                float eventTiming = eventData.Timing * animation.Duration;
                if (animationTimer < eventTiming) continue;
                performedEvents.Add(eventData.EventData);
                foreach (IAnimationEvent animationEvent in animationEvents)
                {
                    animationEvent.ExecuteEvent(eventData.EventData);
                }
            }
        }

        public bool FlipY(bool? value = null)
        {
            bool oldValue = renderer.flipY;
            renderer.flipY = value ?? !renderer.flipY;
            return renderer.flipY != oldValue;
        }
        public bool FlipX(bool? value = null)
        {
            bool oldValue = renderer.flipX;
            renderer.flipX = value ?? !renderer.flipX;
            return renderer.flipX != oldValue;
        }

        public void SetState(string stateName, bool force = false)
        {
            if (currentAnimationState.Name == stateName) force = false;
            previousAnimationState = currentAnimationState;
            currentAnimationState = animationStates[stateName];
            nextAnimationState = currentAnimationState.OutStateName == "" ? null : animationStates[currentAnimationState.OutStateName];
            if (force) animationTimer = 0;
        }

        public void SetNextState(string nextStateName)
        {
            if (nextAnimationState.Name == nextStateName) return;
            nextAnimationState = animationStates[nextStateName];
        }

        public string GetState() => currentAnimationState.Name;

#if UNITY_EDITOR
        private async void OnValidate()
        {
            await System.Threading.Tasks.Task.Delay(10);
            if (this == null) return;
            Animation2D animation = controller?[0]?.Animation;
            bool isNull = animation == null
                || animation.Frames == null
                || animation.Frames.Length <= 0
                || animation.Frames[0] == null;
            GetComponentInChildren<SpriteRenderer>().sprite = isNull ? null : animation.Frames[0].Sprite;
        }
#endif
    }

    public interface IAnimationEvent
    {
        public void ExecuteEvent(AnimationEventData eventData);

        [System.Serializable]
        public class AnimationEventData
        {
            public int Hash => hash;
            public string Name => name;
            public float Value => value;

            [SerializeField] string name;
            [SerializeField, ReadOnly] int hash;
            [SerializeField] float value;

            public AnimationEventData(string name)
            {
                this.name = name;
            }

            public void ComputeHash() => hash = Animator.StringToHash(Name);

            public bool IsEvent(string name) => Animator.StringToHash(name) == Hash;

            public void SetName(ChangeEvent<string> evt)
            {
                name = evt.newValue;
            }

            public static implicit operator int(AnimationEventData data) => data.Hash;
        }
    }
}
