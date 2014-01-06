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

        public static BoardCoordinate Default = new BoardCoordinate(0, 0);
    }
}