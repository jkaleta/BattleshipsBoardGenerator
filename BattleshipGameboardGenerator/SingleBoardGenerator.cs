using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipGameboardGenerator
{
    class SingleBoardGenerator
    {
        private readonly ShipConfiguration _shipConfiguration;
        private readonly IDictionary<int, ShipPositioningParameters> _shipPositioningParametersPerShipLevel;

        public SingleBoardGenerator(ShipConfiguration shipConfiguration)
        {
            _shipConfiguration = shipConfiguration;
            _shipPositioningParametersPerShipLevel = new Dictionary<int, ShipPositioningParameters>(shipConfiguration.ShipLengths.Length);
        }

        public Board GenerateBoard(ShipPositioningParameters[] shipPositioningParameters)
        {
            var workingSet = new CellStatus[10, 10];
            PlaceShips(workingSet, shipPositioningParameters);
            return ConvertWorkingSetIntoBoard(workingSet);
        }

        private void PlaceShips(CellStatus[,] workingSet, ShipPositioningParameters[] shipPositioningParameters)
        {
            var shipsPositionedSoFar = _shipPositioningParametersPerShipLevel.Count;

            if (shipsPositionedSoFar == _shipConfiguration.ShipLengths.Length)
            {
                return;
            }

            var lengthOfNextShipToPosition = _shipConfiguration.ShipLengths[shipsPositionedSoFar];
            var directionOfPositioning = shipPositioningParameters[shipsPositionedSoFar].ShipDirection;
            var startingCoordinate = shipPositioningParameters[shipsPositionedSoFar].ShipCoordinate;

            try
            {
                var position = FindNextFreeSpace(workingSet, startingCoordinate, directionOfPositioning,
                                                 lengthOfNextShipToPosition);
                // actual positioning of the ship
                PlaceShipOnMap(workingSet, lengthOfNextShipToPosition, directionOfPositioning, position);
                PlaceShips(workingSet, shipPositioningParameters);
            }
            catch
            {
                var position = FindNextFreeSpace(workingSet, BoardCoordinate.Default, directionOfPositioning,
                                                 lengthOfNextShipToPosition);
                // actual positioning of the ship
                PlaceShipOnMap(workingSet, lengthOfNextShipToPosition, directionOfPositioning, position);
                PlaceShips(workingSet, shipPositioningParameters);
            }
        }

        private void PlaceShipOnMap(CellStatus[,] workingSet, int lengthOfNextShipToPosition, ShipDirection directionOfPositioning, BoardCoordinate position)
        {
            // first mark everything on and around cells where the ship will be as unavailable
            var startingCoordinateX = Math.Max(position.Column - 1, 0);
            var startingCoordinateY = Math.Max(position.Row - 1, 0);
            var endCoordinateX = directionOfPositioning == ShipDirection.Horizontal ?
                Math.Min(position.Column + lengthOfNextShipToPosition, 9) :
                Math.Min(position.Column + 1, 9);
            var endCoordinateY = directionOfPositioning == ShipDirection.Vertical ?
                Math.Min(position.Row + lengthOfNextShipToPosition, 9) :
                Math.Min(position.Row + 1, 9);

            for (var row = startingCoordinateY; row <= endCoordinateY; row++)
                for (var column = startingCoordinateX; column <= endCoordinateX; column++)
                    workingSet[row, column] = CellStatus.NotAvailableForPlacement;

            Debug.WriteLine(RepresentWorkingSet(workingSet));

            // next mark where the ship actually is
            for (int i = 0; i < lengthOfNextShipToPosition; i++)
            {
                var row = directionOfPositioning == ShipDirection.Horizontal ? position.Row : Math.Min(position.Row + i, 9);
                var column = directionOfPositioning == ShipDirection.Vertical ? position.Column : Math.Min(position.Column + i, 9);

                workingSet[row, column] = CellStatus.ShipPart;
            }

            // finally, add the positioned ship to the collection of positioned ships
            var shipsPositionedSoFar = _shipPositioningParametersPerShipLevel.Count;
            var currentlyPositionedShipPositioningParameters = new ShipPositioningParameters
            {
                ShipCoordinate = position,
                ShipDirection = directionOfPositioning
            };
            _shipPositioningParametersPerShipLevel.Add(shipsPositionedSoFar, currentlyPositionedShipPositioningParameters);

            Debug.WriteLine(RepresentWorkingSet(workingSet));
        }

        private BoardCoordinate FindNextFreeSpace(CellStatus[,] workingSet, BoardCoordinate cellToStartSearch, ShipDirection direction, int shipLength)
        {
            // the free space must be a cluster of cells with 'Free' status, such that
            // they can be contiguous and none of the cells of the new ship lands on 
            // a 'not available for placement' or 'ship part' field.
            for (var row = cellToStartSearch.Row; row < 10; row++)
            {
                for (var column = cellToStartSearch.Column; column < 10; column++)
                {
                    if (workingSet[row, column] == CellStatus.Free)
                    {
                        if (direction == ShipDirection.Horizontal)
                        {
                            // first - check if there is enough room to place the ship in the current row/column
                            if (column + shipLength > 9)
                                continue;

                            // next, check if the entire span of the next few cells is available
                            var spanAvailable = true;
                            for (var c = column; c < column + shipLength; c++)
                            {
                                if (workingSet[row, c] != CellStatus.Free)
                                {
                                    spanAvailable = false;
                                    break;
                                }
                            }
                            if (!spanAvailable)
                                continue;
                        }
                        else
                        {
                            if (row + shipLength > 9)
                                continue;

                            var spanAvailable = true;
                            for (var r = row; r < row + shipLength; r++)
                            {
                                if (workingSet[r, column] != CellStatus.Free)
                                {
                                    spanAvailable = false;
                                    break;
                                }
                            }
                            if (!spanAvailable)
                                continue;
                        }

                        return new BoardCoordinate(row, column);
                    }
                }
            }

            throw new Exception("Could not find a free field in the entire table!");
        }

        private Board ConvertWorkingSetIntoBoard(CellStatus[,] workingSet)
        {
            var generated = new Board(_shipConfiguration);

            workingSet.Foreach((row, column) =>
            {
                if (workingSet[row, column] == CellStatus.ShipPart)
                    generated.BoardRepresentation.Add(new BoardCoordinate(row, column));
            });

            if (!generated.IsValid)
            {
                throw new Exception("Generated board is invalid!");
            }

            return generated;
        }

        private string RepresentWorkingSet(CellStatus[,] workingSet)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < workingSet.GetLength(0); i++)
            {
                for (var j = 0; j < workingSet.GetLength(1); j++)
                {
                    switch (workingSet[i, j])
                    {
                        case CellStatus.Free:
                            sb.Append("_");
                            break;
                        case CellStatus.NotAvailableForPlacement:
                            sb.Append("0");
                            break;
                        case CellStatus.ShipPart:
                            sb.Append("X");
                            break;
                    }
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
