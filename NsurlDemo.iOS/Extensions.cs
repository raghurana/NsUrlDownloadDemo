using System.Collections.Generic;
using Cirrious.FluentLayouts.Touch;
using UIKit;

namespace NsurlDemo.iOS
{
    public static class Extensions
    {
        public static IEnumerable<FluentLayout> VerticalStackPanelConstraints(this UIView parentView, Margins margins,
            params UIView[] views)
        {
            margins = margins ?? new Margins();

            UIView previous = null;
            foreach (var view in views)
            {
                yield return
                    view.Left()
                        .EqualTo()
                        .LeftOf(parentView)
                        .Plus(margins.Left);

                yield return
                    view.Width()
                        .EqualTo()
                        .WidthOf(parentView)
                        .Minus(margins.Right + margins.Left);

                if (previous != null)
                    yield return
                        view.Top()
                            .EqualTo()
                            .BottomOf(previous)
                            .Plus(margins.Top);

                else
                    yield return
                        view.Top()
                            .EqualTo()
                            .TopOf(parentView)
                            .Plus(margins.Top);

                previous = view;
            }

            if (parentView is UIScrollView)
                yield return
                    previous
                        .Bottom()
                        .EqualTo()
                        .BottomOf(parentView)
                        .Minus(margins.Bottom);
        }
    }
}