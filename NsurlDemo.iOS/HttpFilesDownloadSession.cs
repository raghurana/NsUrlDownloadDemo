#region Using Statements

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using Foundation;
using UIKit;

#endregion

namespace NsurlDemo.iOS
{
	public class HttpFilesDownloadSession : NSUrlSessionDownloadDelegate
	{
		#region Fields

		private readonly NSUrlSession downloadSession;
		private readonly Dictionary<nuint, DownloadTaskInfo> downloadTasks;

        #endregion

        #region Event Delegates

        public delegate void SuccessHandler(ushort index, string filePath);

        public delegate void FailureHandler(ushort index, NSError error);

	    public delegate void ProgressHandler(ushort index, float progressValue);

        #endregion

        #region Events

        public event SuccessHandler OnFileDownloadedSuccessfully = (i,f) => {};

		public event FailureHandler OnFileDownloadFailed = (i, e) => { };

        public event ProgressHandler OnFileDownloadProgress = (i,p) => {};

        #endregion

        #region Public Properties

        public ObservableCollection<DownloadFileInfo> DownloadQueue
	    {
	        get; private set; 
        }

		#endregion

		#region Constructors

		public HttpFilesDownloadSession(string uniqueSessionId)
		{
			var config      = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(uniqueSessionId);

			downloadSession = NSUrlSession.FromConfiguration(config, this, new NSOperationQueue());
			downloadTasks   = new Dictionary<nuint, DownloadTaskInfo>();

			DownloadQueue   = new ObservableCollection<DownloadFileInfo>();
			DownloadQueue.CollectionChanged += DownloadQueueOnCollectionChanged;
		}

		#endregion

		#region Public Methods

	    public override void DidWriteData(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
	    {
	        if (downloadTasks.ContainsKey(downloadTask.TaskIdentifier))
	        {
	            var index = downloadTasks[downloadTask.TaskIdentifier].Index;
	            var progress = totalBytesWritten/(float) totalBytesExpectedToWrite;

	            OnFileDownloadProgress(index, progress);
	        }
	    }

	    public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
	    {
	        if (!downloadTasks.ContainsKey(downloadTask.TaskIdentifier))
	            return;

			var cachedTaskInfo  = downloadTasks[downloadTask.TaskIdentifier];

			try
			{
                OnFileDownloadProgress(cachedTaskInfo.Index, 100f);

                var tmpLocation = location.Path;
				var dirName     = Path.GetDirectoryName(cachedTaskInfo.DestinationDiskPath);

				if (!Directory.Exists(dirName))
					Directory.CreateDirectory(dirName);

				if (File.Exists(cachedTaskInfo.DestinationDiskPath))
					File.Delete(cachedTaskInfo.DestinationDiskPath);

				File.Move(tmpLocation, cachedTaskInfo.DestinationDiskPath);
                OnFileDownloadedSuccessfully(cachedTaskInfo.Index, cachedTaskInfo.DestinationDiskPath);
                CleanUpCachedTask(cachedTaskInfo);
            }

			catch (Exception exception)
			{
				OnError(cachedTaskInfo, new NSError(new NSString(exception.Message), 1));
			}
		}

		public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
		{
		    if (error != null)
		    {
		        OnError(downloadTasks[task.TaskIdentifier], error);
		    }
		}

		public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
		{
            Logger.Log(string.Format("DidFinishEventsForBackgroundSession called for session {0}", session.Configuration.Identifier));

            var appDelegate = (AppDelegate) UIApplication.SharedApplication.Delegate;

            // call completion handler when you're done
            if (appDelegate.BgSessionCompletionHandler != null)
            {
                var handler = appDelegate.BgSessionCompletionHandler;
                appDelegate.BgSessionCompletionHandler = null;
                handler.Invoke();
            }
        }

		#endregion

		#region Private Methods

		private void DownloadQueueOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add)
			{
				foreach (DownloadFileInfo addedInfo in e.NewItems)
				{
					var downloadTask = downloadSession.CreateDownloadTask(NSUrl.FromString(addedInfo.FileHttpUrl));

					downloadTasks.Add(downloadTask.TaskIdentifier,
						new DownloadTaskInfo
						{
							Index = addedInfo.Index,
							Task = downloadTask,
							DestinationDiskPath = addedInfo.DestinationDiskPath
						});

					downloadTask.Resume();
				}
			}
		}

		private void OnError(DownloadTaskInfo taskInfo, NSError error)
		{
			OnFileDownloadFailed(taskInfo.Index, error);
		    CleanUpCachedTask(taskInfo);
		}

	    private void CleanUpCachedTask(DownloadTaskInfo cachedTask)
	    {
            downloadTasks.Remove(cachedTask.Task.TaskIdentifier);
            cachedTask.Task.Dispose();
	        cachedTask = null;
	    }

        #endregion
	}
}