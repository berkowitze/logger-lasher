using UnityEngine;

namespace Etienne.Demo
{
    using Animator2D;

    public class DemoPlayer : MonoBehaviour, IAnimationEvent
    {
        [SerializeField] private Animator2D animator;
        private string idleState = "Idle";
        private string jumpState = "Jump";
        private string walkState = "Walk";
        private string runState = "Run";

        public void ExecuteEvent(IAnimationEvent.AnimationEventData eventData)
        {
            Debug.Log($"Event {eventData.Name} executed", this);
        }

        public void SetIdle()
        {
            animator.SetState(idleState);
        }

        public void SetJump()
        {
            animator.SetState(jumpState, true);
        }

        public void SetRun()
        {
            animator.SetState(runState);
        }

        public void SetWalk()
        {
            animator.SetState(walkState);
        }

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space)) SetJump();
        }
    }
}