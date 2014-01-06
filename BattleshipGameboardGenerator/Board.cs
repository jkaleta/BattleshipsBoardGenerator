using System;
using System.Collections.Generic;

namespace BattleshipGameboardGenerator
{
    public class Board
    {
        public Board(ShipConfiguration shipConfiguration)
        {
            BoardRepresentation = new HashSet<BoardCoordinate>();
            ShipConfiguration = shipConfiguration;
        }

        public HashSet<BoardCoordinate> BoardRepresentation { get; private set; }
        public ShipConfiguration ShipConfiguration { get; private set; }

        public bool IsValid
        {
            get
            {
                // TODO - implement 
                return true;
            }
        }
    }
}