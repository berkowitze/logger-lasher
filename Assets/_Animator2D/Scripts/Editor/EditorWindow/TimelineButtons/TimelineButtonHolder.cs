using UnityEngine.UIElements;

namespace EtienneEditor.Animator2D
{
    public class TimelineButtonHolder : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<TimelineButtonHolder> { }

        public TimelineButton FirstButton => firstButton;
        public TimelineButton PreviousButton => previousButton;
        public TimelineButton PlayButton => playButton;
        public TimelineButton NextButton => nextButton;
        public TimelineButton LastButton => lastButton;

        private TimelineButton firstButton, previousButton, playButton, nextButton, lastButton;

        public TimelineButtonHolder()
        {
            style.flexDirection = FlexDirection.Row;

            firstButton = new TimelineButton("Animation.FirstKey.png");
            previousButton = new TimelineButton("Animation.PrevKey.png");
            playButton = new TimelineButton("Animation.Play.png", "Animation.Pause.png");
            nextButton = new TimelineButton("Animation.NextKey.png");
            lastButton = new TimelineButton("Animation.LastKey.png");

            Add(firstButton);
            Add(previousButton);
            Add(playButton);
            Add(nextButton);
            Add(lastButton);
        }
    }
}
