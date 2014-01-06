using System;

namespace BattleshipGameboardGenerator
{
    public static class Util
    {
        public static void Foreach<T>(this T[,] array, Action<int, int> action)
        {
            for (var i = 0; i < array.GetLength(0); i++)
            {
                for (var j = 0; j < array.GetLength(1); j++)
                {
                    action(i, j);
                }
            }
        }
        
    }
}
