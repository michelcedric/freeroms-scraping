using System;

namespace FreeromsScraping
{
    public static class Logger
    {
        public static void Info(string message)
        {
            Log(message, ConsoleColor.White);
        }

        public static void Warning(string message)
        {
            Log(message, ConsoleColor.Yellow);
        }

        public static void Error(string message)
        {
            Log(message, ConsoleColor.Red);
        }

        public static void Log(string message, ConsoleColor foregroundColor)
        {
            var currentForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);
            Console.ForegroundColor = currentForegroundColor;
        }
    }
}
