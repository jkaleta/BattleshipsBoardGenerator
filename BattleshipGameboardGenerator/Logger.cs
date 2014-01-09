using System;

namespace BattleshipGameboardGenerator
{
    public static class Logger
    {
        public static void Log(string format, params object[] parameters)
        {
            if (!Enabled)
                return;

            Console.Out.WriteLine(format, parameters);
        }

        public static bool Enabled { get; set; }
    }
}