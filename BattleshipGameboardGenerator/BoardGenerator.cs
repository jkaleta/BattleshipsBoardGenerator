using System;
using System.Collections.Generic;

namespace BattleshipGameboardGenerator
{
    public class BoardGenerator
    {
        public Board GenerateBoard(ShipConfiguration shipConfiguration)
        {
            var board = new Board(shipConfiguration);

            foreach (var ship in shipConfiguration.ShipLengths)
                PlaceShipOnBoard(board, ship);

            return board;
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
                    var coordinate = new BoardCoordinate((short) row, (short) (column + i));
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
                    var coordinate = new BoardCoordinate((short) (row + 1), (short) column);
                    board.BoardRepresentation.Add(coordinate);
                    Console.Out.WriteLine("Placing mast at {0}", coordinate.ToString());
                }
            }
        }
    }

    public class Board
    {
        public Board(ShipConfiguration shipConfiguration)
        {
            BoardRepresentation = new HashSet<BoardCoordinate>();
            ShipConfiguration = shipConfiguration;
        }

        public HashSet<BoardCoordinate> BoardRepresentation { get; private set; }
        public ShipConfiguration ShipConfiguration { get; private set; }

        public bool IsValid
        {
            get { throw new NotImplementedException(); }
        }
    }

    public class ShipConfiguration
    {
        public ShipConfiguration(int[] shipLengths)
        {
            ShipLengths = shipLengths;
        }

        public int[] ShipLengths { get; private set; }
    }

}
