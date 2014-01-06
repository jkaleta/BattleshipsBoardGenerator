namespace BattleshipGameboardGenerator
{
    public class ShipConfiguration
    {
        public ShipConfiguration(int[] shipLengths)
        {
            ShipLengths = shipLengths;
        }

        public int[] ShipLengths { get; private set; }
    }
}