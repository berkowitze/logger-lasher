using UnityEngine;

namespace Etienne.Animator2D
{
    public class AnimatorController2D : ScriptableObject
    {
        public AnimationState2D[] AnimationStates => animationStates;

        [SerializeField] AnimationState2D[] animationStates;

        public AnimationState2D this[int index] => animationStates[index];

        [System.Serializable]
        public class AnimationState2D
        {
            public string Name => name;
            public Animation2D Animation => animation;
            public bool IsLooping => isLooping;
            public string OutStateName => outStateName;

            [SerializeField] string name;
            [SerializeField] Animation2D animation;
            [SerializeField] bool isLooping = false;
            [SerializeField] string outStateName;
        }
    }
}
