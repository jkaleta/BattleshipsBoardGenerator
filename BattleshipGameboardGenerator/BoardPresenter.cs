using System;
using System.Collections.Generic;
using System.Text;

namespace BattleshipGameboardGenerator
{
    public class BoardPresenter
    {
        public const char ShipRepresentationCharacter = '\u2610';
        public const char EmptyFieldRepresentationCharacter = ' ';

        public string PresentBoardGraphically(Board board, 
            char emptyFieldRepresentation = EmptyFieldRepresentationCharacter,
            char shipRepresentation = ShipRepresentationCharacter)
        {
            var boardRepresentation = new char[10, 10];

            foreach (var element in board.BoardRepresentation)
            {
                boardRepresentation[element.Row, element.Column] = shipRepresentation;
            }

            var sb = new StringBuilder();

            for (var row = 0; row < 10; row++)
            {
                for (var column = 0; column < 10; column++)
                {
                    sb.Append(boardRepresentation[row, column] == shipRepresentation
                                  ? shipRepresentation
                                  : emptyFieldRepresentation);
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
