using System;
using System.Linq;

namespace BattleshipGameboardGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var shipConfiguration = new ShipConfiguration(new[] {4, 3, 3, 2, 2, 2, 1, 1, 1, 1});
            //var boardGenerator = new RandomBoardGenerationStrategy(shipConfiguration);
            var boardGenerator = new BruteForceBoardGenerationStrategy(shipConfiguration);
            boardGenerator.GenerateBoards();
            Console.ReadLine();
        }
    }
}
