using System;
using System.Collections.Generic;
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

        public bool IsValid
        {
            get { return BoardRepresentation.Count == GameConfiguration.ShipLengths.Sum(); }
        }

        public override string ToString()
        {
            return string.Join(",", BoardRepresentation);
        }

        public string PresentBoardGraphically(char shipRepresentation = ShipRepresentationCharacter,
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
    }
}