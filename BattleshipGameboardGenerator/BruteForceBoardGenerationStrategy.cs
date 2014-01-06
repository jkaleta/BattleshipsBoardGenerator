using System;
using System.Collections.Generic;

namespace BattleshipGameboardGenerator
{
    public class BruteForceBoardGenerationStrategy : IBoardGenerationStrategy
    {
        private readonly ShipConfiguration _shipConfiguration;
        private readonly BoardCoordinate _startingCoordinate;
        private readonly IDictionary<int, ShipPositioningParameters> _shipPositioningParametersPerShipLevel;
        private readonly CellStatus[,] _workingSet = new CellStatus[10, 10];

        public BruteForceBoardGenerationStrategy(ShipConfiguration shipConfiguration, BoardCoordinate startingCoordinate)
        {
            _shipConfiguration = shipConfiguration;
            _startingCoordinate = startingCoordinate;
            _shipPositioningParametersPerShipLevel = new Dictionary<int, ShipPositioningParameters>(shipConfiguration.ShipLengths.Length);
        }

        public IEnumerable<Board> GenerateBoards()
        {

            ReInitializeWorkingSet();
            PlaceNextShip();
            yield return ConvertWorkingSetIntoBoard();

        }

        private void PlaceNextShip()
        {
            var shipsPositionedSoFar = _shipPositioningParametersPerShipLevel.Count;

            if (shipsPositionedSoFar == _shipConfiguration.ShipLengths.Length)
            {
                // all ships have been positioned. Iterate to next board. 
                // TODO
                return;
            }

            var lengthOfNextShipToPosition = _shipConfiguration.ShipLengths[shipsPositionedSoFar];
            var directionOfPositioning = ShipDirection.Horizontal; // TODO
            var position = FindNextFreeSpace(_startingCoordinate, directionOfPositioning, lengthOfNextShipToPosition);
            // TODO - the starting coordinate must be extracted from per level stuff

            // actual positioning of the ship
            PlaceShipOnMap(lengthOfNextShipToPosition, directionOfPositioning, position);

            // TODO

            PlaceNextShip();
        }

        private void PlaceShipOnMap(int lengthOfNextShipToPosition, ShipDirection directionOfPositioning, BoardCoordinate position)
        {
            Console.Out.WriteLine("Placing ship on map: {0} masts, position: {1}", lengthOfNextShipToPosition, position);

            // first mark everything on and around cells where the ship will be as unavailable
            var startingCoordinateX = Math.Max(position.Column - 1, 0);
            var startingCoordinateY = Math.Max(position.Row - 1, 0);
            var endCoordinateX = directionOfPositioning == ShipDirection.Horizontal ?
                Math.Min(position.Column + lengthOfNextShipToPosition + 1, 9) :
                Math.Min(position.Column + 1, 9);
            var endCoordinateY = directionOfPositioning == ShipDirection.Vertical ?
                Math.Min(position.Row + lengthOfNextShipToPosition + 1, 9) :
                Math.Min(position.Row + 1, 9);

            for (var row = startingCoordinateY; row <= endCoordinateY; row++)
                for (var column = startingCoordinateX; column <= endCoordinateX; column++)
                    _workingSet[row, column] = CellStatus.NotAvailableForPlacement;

            // next mark where the ship actually is
            for (int i = 0; i < lengthOfNextShipToPosition; i++)
            {
                var row = directionOfPositioning == ShipDirection.Horizontal ? position.Row : Math.Min(position.Column + i, 9);
                var column = directionOfPositioning == ShipDirection.Vertical ? position.Column : Math.Min(position.Column + i, 9);

                _workingSet[row, column] = CellStatus.ShipPart;
            }

            // finally, add the positioned ship to the collection of positioned ships
            var shipsPositionedSoFar = _shipPositioningParametersPerShipLevel.Count;
            var currentlyPositionedShipPositioningParameters = new ShipPositioningParameters
                {
                    LastPositionedShipCoordinate = position,
                    LastShipDirection = directionOfPositioning
                };
            _shipPositioningParametersPerShipLevel.Add(shipsPositionedSoFar, currentlyPositionedShipPositioningParameters);
        }

        private BoardCoordinate FindNextFreeSpace(BoardCoordinate cellToStartSearch, ShipDirection direction, int shipLength)
        {
            // the free space must be a cluster of cells with 'Free' status, such that
            // they can be contiguous and none of the cells of the new ship lands on 
            // a 'not available for placement' or 'ship part' field. 

            // TODO - overflow to start search from 0 when cannot find place in the last column

            for (var row = cellToStartSearch.Row; row < 10; row++)
            {
                for (var column = cellToStartSearch.Column; column < 10; column++)
                {
                    if (_workingSet[row, column] == CellStatus.Free)
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
                                if (_workingSet[row, c] != CellStatus.Free)
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
                                if (_workingSet[r, column] != CellStatus.Free)
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

        private Board ConvertWorkingSetIntoBoard()
        {
            var generated = new Board(_shipConfiguration);

            _workingSet.Foreach((row, column) =>
                {
                    if (_workingSet[row, column] == CellStatus.ShipPart)
                        generated.BoardRepresentation.Add(new BoardCoordinate(row, column));
                });

            if (!generated.IsValid)
            {
                throw new Exception("Generated board is invalid!");
            }

            return generated;
        }

        private void ReInitializeWorkingSet()
        {
            _workingSet.Foreach((row, c) => _workingSet[row, c] = CellStatus.Free);
        }
    }

    internal class ShipPositioningParameters
    {
        public BoardCoordinate LastPositionedShipCoordinate { get; set; }
        public ShipDirection LastShipDirection { get; set; }
    }

    internal enum ShipDirection
    {
        Horizontal,
        Vertical
    }

    internal enum CellStatus
    {
        ShipPart,
        NotAvailableForPlacement,
        Free
    }
}
