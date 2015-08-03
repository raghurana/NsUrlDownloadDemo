using System;

namespace NsurlDemo.iOS
{
    public static class Logger
    {
        public static void Log(string message)
        {
            Console.WriteLine("NSURLDEMO ---> {0}", message);
        }
    }
}