using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BattleshipGameboardGenerator
{
    public class Board
    {
        public const char ShipRepresentationCharacter = 'x';
        public const char EmptyFieldRepresentationCharacter = '0';

        public Board(GameConfiguration gameConfiguration)
        {
            BoardRepresentation = new HashSet<BoardCoordinate>();
            GameConfiguration = gameConfiguration;
        }

        public HashSet<BoardCoordinate> BoardRepresentation { get; private set; }
        public GameConfiguration GameConfiguration { get; private set; }

        public override string ToString()
        {
            return string.Join(",", BoardRepresentation);
        }

        public string PresentBoardGraphically(
            char shipRepresentation = ShipRepresentationCharacter,
            char emptyFieldRepresentation = EmptyFieldRepresentationCharacter)
        {
            var boardRepresentation = new char[GameConfiguration.BoardSize, GameConfiguration.BoardSize];

            foreach (var element in BoardRepresentation)
            {
                boardRepresentation[element.Row, element.Column] = ShipRepresentationCharacter;
            }

            var sb = new StringBuilder();

            for (var row = 0; row < GameConfiguration.BoardSize; row++)
            {
                for (var column = 0; column < GameConfiguration.BoardSize; column++)
                {
                    sb.Append(boardRepresentation[row, column] == ShipRepresentationCharacter
                                  ? ShipRepresentationCharacter
                                  : EmptyFieldRepresentationCharacter);
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public string Validate()
        {
            if (BoardRepresentation.Count != GameConfiguration.ShipLengths.Sum())
                return "Sum of masts on the borad is different than masts in game configuration";

            // Detect ships just by grouping all neighbouring board coordinates
            int groupCount;
            Dictionary<BoardCoordinate, int> groupPerCoordinate = FindGroupsOfCoordinates(out groupCount);

            if (groupCount != GameConfiguration.ShipCount)
                return string.Format("Detected {0} mast groups, expected {1} ships");

            // Prep for remaining checks
            var coordinateGroups = CreateCoordinateGroups(groupCount, groupPerCoordinate);

            // Mast counts
            var shipLengths = new List<int>(GameConfiguration.ShipLengths);
            int shipsToFindCount = shipLengths.Count;
            foreach (CoordinateGroup group in coordinateGroups)
            {
                int shipLength = group.Count;
                if (shipLengths.Contains(shipLength))
                {
                    shipLengths.Remove(shipLength);
                    shipsToFindCount--;
                }
            }
            
            if (shipsToFindCount != 0)
                return "Found mast groups which have different number of masts than ships in this game configuration";

            // Ship shape - only vertical or horizontal coordiates allowed
            foreach (CoordinateGroup group in coordinateGroups)
            {
                string result = group.ValidateShape();
                if (result != null)
                    return result;
            }
            return null;
        }

        private CoordinateGroup[] CreateCoordinateGroups(
            int groupCount, 
            Dictionary<BoardCoordinate, int> groupPerCoordinate)
        {
            var coordinateGroups = new CoordinateGroup[groupCount];
            foreach (BoardCoordinate coordinate in groupPerCoordinate.Keys)
            {
                int groupId = groupPerCoordinate[coordinate];
                if (coordinateGroups[groupId] == null)
                    coordinateGroups[groupId] = new CoordinateGroup();

                coordinateGroups[groupId].Add(coordinate);
            }
            return coordinateGroups;
        }

        private Dictionary<BoardCoordinate, int> FindGroupsOfCoordinates(out int groupCount)
        {
            var groupPerCoordinate = new Dictionary<BoardCoordinate, int>();
            groupCount = 0;
            foreach (BoardCoordinate startCoordinate in BoardRepresentation)
            {
                if (groupPerCoordinate.ContainsKey(startCoordinate))
                    continue;

                AddCoordinateAndNeighboursToGroup(startCoordinate, groupPerCoordinate, groupCount);
                groupCount++;
            }
            return groupPerCoordinate;
        }

        private class CoordinateGroup
        {
            private HashSet<BoardCoordinate> _coordinates = new HashSet<BoardCoordinate>();
            public ShipDirection? _direction;

            public int Count
            {
                get
                {
                    return _coordinates.Count;
                }
            }

            public void Add(BoardCoordinate coordinate)
            {
                _coordinates.Add(coordinate);
            }

            public string ValidateShape()
            {
                // Skip ships with single mast
                if (_coordinates.Count == 1)
                    return null;

                foreach (BoardCoordinate coordinate in _coordinates)
                {
                    var verticalNeighbours = new BoardCoordinate[2]
                    {
                        // Top 
                        new BoardCoordinate(coordinate.Row - 1, coordinate.Column),
                        // Bottom
                        new BoardCoordinate(coordinate.Row + 1, coordinate.Column),
                    };

                    var horizontalNeighbours = new BoardCoordinate[2]
                    {
                        // Left side
                        new BoardCoordinate(coordinate.Row, coordinate.Column - 1),
                        // Right side
                        new BoardCoordinate(coordinate.Row, coordinate.Column + 1),
                    };

                    bool vertical = _coordinates.Any(c => verticalNeighbours.Contains(c));
                    bool horizontal = _coordinates.Any(c => horizontalNeighbours.Contains(c));

                    if (!vertical && !horizontal)
                        return "Found neighbour in the corner";

                    if (vertical && horizontal)
                        return "Found both vertical and horizontal neighbour";

                    // If there is no information wheather the group is horizontal or vertical, just check
                    // one of the neighbours first
                    if (_direction.HasValue == false)
                        _direction = vertical ? ShipDirection.Vertical : ShipDirection.Horizontal;

                    if (_direction == ShipDirection.Vertical && horizontal)
                        return "This coordinate has horizontal neighbours, but the rest of the group is vertical";
                    if (_direction == ShipDirection.Horizontal && vertical)
                        return "This coordinate has vertical neighbours, but the rest of the group is horizontal";
                }

                return null;
            }
        }

        private void AddCoordinateAndNeighboursToGroup(
            BoardCoordinate startCoordinate, 
            Dictionary<BoardCoordinate, int> groups,
            int currentGroup)
        {
            groups[startCoordinate] = currentGroup;

            HashSet<BoardCoordinate> possibleNeighbours = new HashSet<BoardCoordinate>();
            for (int i = 0; i < 3; i++)
            {
                // Left side
                possibleNeighbours.Add(new BoardCoordinate(startCoordinate.Row - 1 + i, startCoordinate.Column - 1));
                // Right side
                possibleNeighbours.Add(new BoardCoordinate(startCoordinate.Row - 1 + i, startCoordinate.Column + 1));
                // Top side
                possibleNeighbours.Add(new BoardCoordinate(startCoordinate.Row - 1, startCoordinate.Column - 1 + i));
                // Bottom side
                possibleNeighbours.Add(new BoardCoordinate(startCoordinate.Row + 1, startCoordinate.Column - 1 + i));
            }

            foreach (BoardCoordinate coordinate in possibleNeighbours)
            {
                if (BoardRepresentation.Contains(coordinate) && !groups.ContainsKey(coordinate))
                {
                    AddCoordinateAndNeighboursToGroup(coordinate, groups, currentGroup);
                }
            }
        }
    }
}