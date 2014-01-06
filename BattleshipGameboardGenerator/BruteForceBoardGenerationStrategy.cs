using System;
using System.Collections.Generic;

namespace BattleshipGameboardGenerator
{
    public class BruteForceBoardGenerationStrategy : IBoardGenerationStrategy
    {
        private ShipConfiguration _shipConfiguration;
        private readonly BoardCoordinate _startingCoordinate;
        private IDictionary<int, ShipPositioningParameters> _shipPositioningParametersPerShipLevel;
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
            var position = FindNextFreePosition(_startingCoordinate); // TODO - the starting coordinate must be extracted from per level stuff

            // actual positioning of the ship
            PlaceShipOnMap(lengthOfNextShipToPosition, directionOfPositioning, position);

            // TODO

            PlaceNextShip();
        }

        private void PlaceShipOnMap(int lengthOfNextShipToPosition, ShipDirection directionOfPositioning, BoardCoordinate position)
        {
            Console.Out.WriteLine("Placing ship on map: {0} masts", lengthOfNextShipToPosition);

            // first mark everything on and around cells where the ship will be as unavailable
            var startingCoordinateX = Math.Max(position.Row - 1, 0);
            var startingCoordinateY = Math.Max(position.Column - 1, 0);
            var endCoordinateX = directionOfPositioning == ShipDirection.Horizontal ?
                Math.Min(position.Column + lengthOfNextShipToPosition, 9) :
                Math.Min(position.Column + 1, 9);
            var endCoordinateY = directionOfPositioning == ShipDirection.Horizontal ?
                Math.Min(position.Row + lengthOfNextShipToPosition, 9) :
                Math.Min(position.Row + 1, 9);

            for (var i = startingCoordinateX; i < endCoordinateX; i++)
                for (var j = startingCoordinateY; j < endCoordinateY; j++)
                    _workingSet[i, j] = CellStatus.NotAvailableForPlacement;

            // next mark where the ship actually is
            for (int i = 0; i < lengthOfNextShipToPosition; i++)
            {
                var row = directionOfPositioning == ShipDirection.Horizontal ? Math.Min(position.Row + i, 9) : position.Row;
                var column = directionOfPositioning == ShipDirection.Horizontal ? position.Column : Math.Min(position.Column + i, 9);

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

        private BoardCoordinate FindNextFreePosition(BoardCoordinate cellToStartSearch)
        {
            for (short i = cellToStartSearch.Row; i < 10; i++)
            {
                for (short j = cellToStartSearch.Column; j < 10; j++)
                {
                    if (_workingSet[i, j] == CellStatus.Free)
                    {
                        // TODO -add way more logic here 
                        return new BoardCoordinate(i, j);
                    }
                }
            }

            throw new Exception("Could not find a free field in the entire table!");
        }

        private Board ConvertWorkingSetIntoBoard()
        {
            var generated = new Board(_shipConfiguration);

            _workingSet.Foreach((i, j) =>
                {
                    if (_workingSet[i, j] == CellStatus.ShipPart)
                        generated.BoardRepresentation.Add(new BoardCoordinate((short)i, (short)j));
                });

            if (!generated.IsValid)
            {
                throw new Exception("Generated board is invalid!");
            }

            return generated;
        }

        private void ReInitializeWorkingSet()
        {
            _workingSet.Foreach((i, j) => _workingSet[i, j] = CellStatus.Free);
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
