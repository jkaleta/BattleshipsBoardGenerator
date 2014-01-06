using System;
using System.Collections.Generic;

namespace BattleshipGameboardGenerator
{
    public interface IBoardGenerationStrategy
    {
        IEnumerable<Board> GenerateBoards();
    }

    public class RandomBoardGenerationStrategy : IBoardGenerationStrategy
    {
        private readonly ShipConfiguration _shipConfiguration;

        public RandomBoardGenerationStrategy(ShipConfiguration shipConfiguration)
        {
            _shipConfiguration = shipConfiguration;
        }

        public IEnumerable<Board> GenerateBoards()
        {
            var board = new Board(_shipConfiguration);

            foreach (var ship in _shipConfiguration.ShipLengths)
                PlaceShipOnBoard(board, ship);

            yield return board;
        }

        private void PlaceShipOnBoard(Board board, int shipLength)
        {
            // naive strategy - rand x, rand y, rand direction, no collision detection
            var randomizer = new Random(DateTime.Now.Millisecond);
            var row = randomizer.Next(9);
            var column = randomizer.Next(9);
            var direction = randomizer.Next(1);

            if (direction == 0)
            {
                // horizontally
                if (column > 9 - shipLength)
                    column = 9 - shipLength;

                for (var i = 0; i < shipLength; i++)
                {
                    var coordinate = new BoardCoordinate(row, (column + i));
                    board.BoardRepresentation.Add(coordinate);
                    Console.Out.WriteLine("Placing mast at {0}", coordinate.ToString());
                }
            }
            else
            {
                // vertically
                if (row > 9 - shipLength)
                    row = 9 - shipLength;

                for (var i = 0; i < shipLength; i++)
                {
                    var coordinate = new BoardCoordinate((row + 1), column);
                    board.BoardRepresentation.Add(coordinate);
                    Console.Out.WriteLine("Placing mast at {0}", coordinate.ToString());
                }
            }
        }
    }
}