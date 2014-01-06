using System;

namespace BattleshipGameboardGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var boardGenerator = new BoardGenerator();
            var board = boardGenerator.GenerateBoard(new ShipConfiguration(new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }));
            var presenter = new BoardPresenter();
            var boardPresentation = presenter.PresentBoardGraphically(board, '0', '1');

            Console.Out.Write(boardPresentation);

            Console.ReadLine();
        }
    }
}
