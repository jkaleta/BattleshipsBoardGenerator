using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleshipGameboardGenerator
{
    public class ShipConfiguration
    {
        public ShipConfiguration(int[] shipLengths)
        {
            ShipLengths = shipLengths;
        }

        public int[] ShipLengths { get; private set; }

        public IEnumerable<IEnumerable<ShipPositioningParameters>> GetListOfPossibleCombinationsOfShipPositions()
        {
            var possiblePositionsOnBoard = BoardCoordinate.GetAllPossibleCoordinates(10, 10).ToArray();
            var allHorizontalPositions =
                (from position in possiblePositionsOnBoard.Where(p => p.Row != 5 && p.Column != 2 && p.Row % 2 == 1 && p.Column % 2 == 0)
                 select new ShipPositioningParameters
                     {
                         ShipCoordinate = position,
                         ShipDirection = ShipDirection.Horizontal
                     }).ToArray();
            var allVerticalPositions =
                 (from position in possiblePositionsOnBoard.Where(p => p.Row != 8 && p.Column != 4 && p.Row % 2 == 0 && p.Column % 2 == 1)
                  select new ShipPositioningParameters
                  {
                      ShipCoordinate = position,
                      ShipDirection = ShipDirection.Vertical
                  }).ToArray();

            var allPossiblePositioningParametersForShip = allHorizontalPositions.Union(allVerticalPositions).ToArray();

            var rng = new Random();

            var lists = ShipLengths.Select(t => t > 1 ?
                allPossiblePositioningParametersForShip.Shuffle(new Random(rng.Next())) :
                allHorizontalPositions.Shuffle(new Random(rng.Next()))).ToList();

            return lists.CartesianProduct();
        }
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