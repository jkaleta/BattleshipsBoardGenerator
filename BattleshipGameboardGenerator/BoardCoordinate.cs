using System.Globalization;

namespace BattleshipGameboardGenerator
{
    public struct BoardCoordinate
    {
        public BoardCoordinate(short row, short column)
            : this()
        {
            Row = row;
            Column = column;
        }

        public short Row { get; set; }
        public short Column { get; set; }
        public override string ToString()
        {
            return ((char)((65) + Row)) + (Column + 1).ToString(CultureInfo.InvariantCulture);
        }
    }
}