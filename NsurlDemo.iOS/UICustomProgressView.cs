using Cirrious.FluentLayouts.Touch;
using UIKit;

namespace NsurlDemo.iOS
{
    public class UICustomProgressView : UIView
    {
        private UIView progressNoContainer;
        private UILabel currentProgressLabel;
        private UILabel ofProgressLabel;
        private UILabel totalProgressLabel;

        public UICustomProgressView()
        {
            currentProgressLabel = new UILabel();
            currentProgressLabel.Text = "10000000";

            ofProgressLabel = new UILabel();
            ofProgressLabel.Text = "of";

            totalProgressLabel = new UILabel();
            totalProgressLabel.Text = "10000000";

            progressNoContainer = new UIView();
            progressNoContainer.BackgroundColor = UIColor.Yellow;
            progressNoContainer.Layer.BorderColor = UIColor.Red.CGColor;
            progressNoContainer.Layer.BorderWidth = 2f;

            progressNoContainer.AddSubviews(new UIView[] { currentProgressLabel, ofProgressLabel, totalProgressLabel });
            progressNoContainer.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            progressNoContainer.AddConstraints(new[]
            {
                currentProgressLabel.AtTopOf(progressNoContainer),
                currentProgressLabel.WithSameCenterX(progressNoContainer).Minus(25),                
                currentProgressLabel.Height().EqualTo(30), 

                ofProgressLabel.WithSameTop(currentProgressLabel),
                ofProgressLabel.ToRightOf(currentProgressLabel).Plus(5),
                ofProgressLabel.WithSameHeight(currentProgressLabel),

                totalProgressLabel.WithSameBottom(ofProgressLabel),
                totalProgressLabel.ToRightOf(ofProgressLabel).Plus(5),
                totalProgressLabel.WithSameHeight(ofProgressLabel)
            });

            AddSubviews(new[] { progressNoContainer });
            this.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            this.AddConstraints(new[]
            {
                progressNoContainer.WithSameCenterY(this),
                progressNoContainer.WithSameRight(this).Minus(10),                
                progressNoContainer.Width().EqualTo(100),
                progressNoContainer.Height().EqualTo(30)
            });
        }
    }
}