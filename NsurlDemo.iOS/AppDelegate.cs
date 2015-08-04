using System;
using Foundation;
using UIKit;

namespace NsurlDemo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        private UIWindow window;
        private PollingBackgroundWorker pollingBackgroundWorker;

        public Action BgSessionCompletionHandler;
        public static string BgSessionIdentifier = "com.something.unique";
        
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            pollingBackgroundWorker = new PollingBackgroundWorker();

            window = new UIWindow(UIScreen.MainScreen.Bounds)
            {
                RootViewController = new DownloadsViewController()
            };

            window.MakeKeyAndVisible();

            KickOffForegroundPolling();

            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Logger.Log("Did Enter Background");
            pollingBackgroundWorker.FinishExecution();
            UIApplication
                .SharedApplication
                .SetMinimumBackgroundFetchInterval(2);
        }
        
        public override void WillEnterForeground(UIApplication application)
        {
            Logger.Log("Will Enter Foreground");
            UIApplication
               .SharedApplication
               .SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalNever);

            KickOffForegroundPolling();
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            Console.WriteLine("Performing Background Fetch...");
            completionHandler(UIBackgroundFetchResult.NewData);
        }
        
        public override void HandleEventsForBackgroundUrl(
            UIApplication application, 
            string sessionIdentifier, 
            Action completionHandler)
        {
            Logger.Log("HandleEventsForBackgroundUrl called, we now have a BgSessionCompletionHandler");
            BgSessionCompletionHandler = completionHandler;
        }

        private void KickOffForegroundPolling()
        {
            pollingBackgroundWorker.ExecuteInBackground(() =>
            {
                Console.WriteLine("Performing Foreground Fetch...");
            }, TimeSpan.FromSeconds(2));
        }

    }
}