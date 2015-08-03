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

        public static string BgSessionIdentifier = "com.something.unique";

        public Action BgSessionCompletionHandler;
        
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            window = new UIWindow(UIScreen.MainScreen.Bounds)
            {
                RootViewController = new DownloadsViewController()
            };

            window.MakeKeyAndVisible();

            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            Logger.Log("Did Enter Background");
        }


        public override void HandleEventsForBackgroundUrl(
            UIApplication application, 
            string sessionIdentifier, 
            Action completionHandler)
        {
            Logger.Log("HandleEventsForBackgroundUrl called, we now have a BgSessionCompletionHandler");
            BgSessionCompletionHandler = completionHandler;
        }
    }
}