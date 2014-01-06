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
            var boardGenerator = new BruteForceBoardGenerationStrategy(shipConfiguration, BoardCoordinate.Default);
            var boards = boardGenerator.GenerateBoards().ToList();
            var presenter = new BoardPresenter();

            foreach(var b in boards)
            {
                var boardPresentation = presenter.PresentBoardGraphically(b, '0', '1');
                Console.Out.Write(boardPresentation);
            }

            Console.ReadLine();
        }
    }
}
