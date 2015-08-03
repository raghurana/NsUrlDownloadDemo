using Cirrious.FluentLayouts.Touch;
using UIKit;

namespace NsurlDemo.iOS
{
    public class UIProgressiveImageView : UIView
    {
        private readonly UIImageView imageView;
        private readonly UIProgressView progressView;

        public UIProgressiveImageView()
        {
            imageView = new UIImageView();
            progressView = new UIProgressView(UIProgressViewStyle.Default);            

            Design();
        }

        public UIProgressView ProgressView
        {
            get { return progressView; }
        }

        public UIImageView ImageView
        {
            get { return imageView; }
        }

        private void Design()
        {
            AddSubviews(progressView, imageView);

            this.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();
            this.AddConstraints(new[]
            {
                ProgressView.AtTopOf(this, 10),
                ProgressView.AtLeftOf(this, 10),
                ProgressView.WithSameWidth(this).Minus(20),
                progressView.Height().EqualTo(5), 

                ImageView.Below(ProgressView, 10),
                ImageView.AtLeftOf(this, 10),
                ImageView.WithSameWidth(ProgressView), 
                ImageView.WithSameBottom(this).Minus(10)
            });
        }

    }
}