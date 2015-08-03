#region Using Statements

using Cirrious.FluentLayouts.Touch;
using CoreGraphics;
using Foundation;
using System;
using System.IO;
using UIKit;

#endregion

namespace NsurlDemo.iOS
{
	public class DownloadsViewController : UIViewController
	{
		#region Constants

		private const int TotalViews = 10;
		private const int ViewHeight = 250;

		#endregion

		#region Fields
        
		private UIButton downloadButton;
		private UIScrollView scrollView;
		private HttpFilesDownloadSession session;

        #endregion

		#region Public Methods

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			View.BackgroundColor = UIColor.White;

			downloadButton = UIButton.FromType(UIButtonType.RoundedRect);
			downloadButton.SetTitle("Start Downloading", UIControlState.Normal);
			downloadButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			downloadButton.BackgroundColor = UIColor.Blue;
			downloadButton.Layer.CornerRadius = 10f;
			downloadButton.TouchUpInside += DownloadButtonOnTouchUpInside;

			scrollView = new UIScrollView();

			for (var i = 0; i < TotalViews; i++)
			{
				var view = new UIProgressiveImageView();
				view.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				view.ImageView.BackgroundColor = UIColor.Gray;

				scrollView.AddSubview(view);
				scrollView.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

				if (i == 0)
				{
					scrollView.AddConstraints(new []
					{
						view.AtTopOf(scrollView),
						view.AtLeftOf(scrollView),
						view.WithSameWidth(scrollView),
						view.Height().EqualTo(ViewHeight),
					});
				}

				else
				{
					var previousView = scrollView.Subviews[i - 1];
					scrollView.AddConstraints(new []
					{
						view.Below(previousView),
						view.WithSameLeft(previousView),
						view.WithSameWidth(previousView),
						view.WithSameHeight(previousView)
					});
				}
			}

			View.AddSubviews(downloadButton, scrollView);
			View.SubviewsDoNotTranslateAutoresizingMaskIntoConstraints();

			View.AddConstraints(new[]
			{
				downloadButton.AtTopOf(View, UIApplication.SharedApplication.StatusBarFrame.Height),
				downloadButton.WithSameCenterX(View),
				downloadButton.WithSameWidth(View).Minus(20),
				downloadButton.Height().EqualTo(40),

				scrollView.Below(downloadButton),
				scrollView.AtLeftOf(View),
				scrollView.WithSameWidth(View),
				scrollView.WithSameBottom(View)
			});

			session = new HttpFilesDownloadSession(AppDelegate.BgSessionIdentifier);
			session.OnFileDownloadedSuccessfully += SessionOnFileDownloadedSuccessfully;
			session.OnFileDownloadFailed += SessionOnFileDownloadFailed;
		    session.OnFileDownloadProgress += OnProgress;
		}

		public override void ViewWillLayoutSubviews()
		{
			scrollView.ContentSize = new CGSize(View.Bounds.Width, (ViewHeight * TotalViews) + UIApplication.SharedApplication.StatusBarFrame.Height);
			base.ViewWillLayoutSubviews();
		}

		#endregion

		#region Private Methods

		private void DownloadButtonOnTouchUpInside(object sender, EventArgs eventArgs)
		{
			var libFolder =
						Path.Combine(
							Environment.GetFolderPath(Environment.SpecialFolder.Personal),
							"..",
							"Library",
							"MyApp",
							"Images");

			var download1 = new DownloadFileInfo
			{
				Index = 0,
                //FileHttpUrl = "http://crockerj.com/photos/post23/fatcat10.jpg",
                FileHttpUrl = "http://drpinna.com/wp-content/uploads/2010/02/fat-cat1.jpg",
                DestinationDiskPath = Path.Combine(libFolder, "fatCat1.jpeg")
			};

			var download2 = new DownloadFileInfo
			{
				Index = 1,
				FileHttpUrl = "http://drpinna.com/wp-content/uploads/2010/02/fat-cat1.jpg",
				DestinationDiskPath = Path.Combine(libFolder, "fatCat2.jpeg")
			};

			var download3 = new DownloadFileInfo
			{
				Index = 2,
                //FileHttpUrl = "http://s3.favim.com/orig/42/cat-fat-cat-kitty-pursia-Favim.com-358172.jpg",
                FileHttpUrl = "http://drpinna.com/wp-content/uploads/2010/02/fat-cat1.jpg",
                DestinationDiskPath = Path.Combine(libFolder, "fatCat3.jpeg")
			};

            var download4 = new DownloadFileInfo
            {
                Index = 3,
                //FileHttpUrl = "http://www.zastavki.com/pictures/originals/2013/Animals___Cats__fat_cat_043905_.jpg",
                FileHttpUrl = "http://drpinna.com/wp-content/uploads/2010/02/fat-cat1.jpg",
                DestinationDiskPath = Path.Combine(libFolder, "fatCat4.jpg")
            };

            var download5 = new DownloadFileInfo
            {
                Index = 4,
                //FileHttpUrl = "http://images4.fanpop.com/image/photos/15700000/Honey-and-her-favorite-toy-Mr-Chipmunk-honey-the-fat-cat-15761646-2560-1920.jpg",
                FileHttpUrl = "http://drpinna.com/wp-content/uploads/2010/02/fat-cat1.jpg",
                DestinationDiskPath = Path.Combine(libFolder, "fatCat5.jpeg")
            };

            session.DownloadQueue.Add(download1);
			session.DownloadQueue.Add(download2);
			session.DownloadQueue.Add(download3);
			session.DownloadQueue.Add(download4);
			session.DownloadQueue.Add(download5);
		}

		private void OnProgress(ushort index, float progressValue)
		{
            InvokeOnMainThread(() =>
		    {
		        if (index < scrollView.Subviews.Length)
		        {
		            var targetView = (UIProgressiveImageView) scrollView.Subviews[index];
		            targetView.ProgressView.Progress = progressValue;
		        }
		    });
		}

		private void SessionOnFileDownloadedSuccessfully(ushort index, string filePath)
		{
            Logger.Log($"Download at {index} was successful.");

            InvokeOnMainThread(() =>
			{
			    if (index < scrollView.Subviews.Length)
			    {
			        var targetView = (UIProgressiveImageView) scrollView.Subviews[index];
                    targetView.ImageView.Image  = UIImage.FromFile(filePath);
			    }
			});
		}

		private void SessionOnFileDownloadFailed(ushort index, NSError error)
		{
		    string title = $"Download at index {index} failed.";
            Logger.Log($"{title} {error.DebugDescription}");

		    InvokeOnMainThread(() =>
		    {
		        var okAlertController =
		            UIAlertController.Create(title, error.Description, UIAlertControllerStyle.Alert);

		        okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, a =>
		        {
		        }));

		        PresentViewController(okAlertController, true, () => { });
		    });
		}

		#endregion
	}
}