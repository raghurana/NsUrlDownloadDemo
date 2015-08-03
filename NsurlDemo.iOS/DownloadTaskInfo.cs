using Foundation;

namespace NsurlDemo.iOS
{
    public class DownloadTaskInfo
    {
        public ushort Index { get; set; }

        public NSUrlSessionDownloadTask Task { get; set; }

        public string DestinationDiskPath { get; set; }
    }
}