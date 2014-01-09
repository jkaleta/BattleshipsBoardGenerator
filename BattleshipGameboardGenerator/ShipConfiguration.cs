namespace BattleshipGameboardGenerator
{
    public class GameConfiguration
    {
        public GameConfiguration(string friendlyName, int[] shipLengths, int boardSize)
        {
            FriendlyName = friendlyName;
            ShipLengths = shipLengths;
            ShipCount = ShipLengths.Length;
            BoardSize = boardSize;
        }

        public string FriendlyName { get; set; }
        public int[] ShipLengths { get; private set; }
        public int ShipCount { get; private set; }
        public int BoardSize { get; private set; }

        public static GameConfiguration RussianConfiguration = new GameConfiguration("Russian Configuration", new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }, 10);
        public static GameConfiguration MiltonBradleyConfiguration = new GameConfiguration("Milton Bradley Configuration", new[] { 5, 4, 3, 3, 2}, 10);
    }

    public class ShipPositioningParameters
    {
        public BoardCoordinate ShipCoordinate { get; set; }
        public ShipDirection ShipDirection { get; set; }

        public override string ToString()
        {
            return ShipCoordinate.ToString() + "(" + ShipDirection.ToString().Substring(0, 1) + ")";
        }
    }

    public enum ShipDirection
    {
        Horizontal,
        Vertical
    }
}