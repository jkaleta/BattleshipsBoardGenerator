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

            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    sb.Append(boardRepresentation[i, j] == shipRepresentation
                                  ? shipRepresentation
                                  : emptyFieldRepresentation);
                }
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }
    }
}
