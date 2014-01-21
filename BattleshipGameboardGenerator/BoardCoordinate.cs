using System;
using System.Collections.Generic;
using System.Globalization;

namespace BattleshipGameboardGenerator
{
    public struct BoardCoordinate
    {
        public BoardCoordinate(int row, int column)
            : this()
        {
            Row = row;
            Column = column;
        }

        public int Row { get; set; }
        public int Column { get; set; }
        public override string ToString()
        {
            return ((char)((65) + Row)) + (Column + 1).ToString(CultureInfo.InvariantCulture);
        }

        public static IEnumerable<BoardCoordinate> GetAllPossibleCoordinates(int xDim, int yDim)
        {
            for (int i = 0; i < xDim; i++)
                for (int j = 0; j < yDim; j++)
                    yield return new BoardCoordinate(i, j);
        }

        public static BoardCoordinate Default = new BoardCoordinate(0, 0);

        public static BoardCoordinate GetRandomCoordinateInFirstQuadrant()
        {
            return new BoardCoordinate(new Random(DateTime.Now.Millisecond).Next() % 4, new Random(DateTime.Now.Millisecond).Next() % 4);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BoardCoordinate))
                return false;

            BoardCoordinate other = (BoardCoordinate)obj;
            return Row == other.Row && Column == other.Column;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Row;
                hash = hash * 23 + Column;
                return hash;
            }
        }
    }
}