using System;
using System.Collections.Generic;
using System.Linq;

namespace BattleshipGameboardGenerator
{
    public class ShipPositioningParametersGenerator
    {
        private readonly GameConfiguration _gameConfiguration;

        /// <summary>
        /// Initializes a new instance of ShipPositioningParametersGenerator
        /// </summary>
        /// The number of groups should be greater or equal to the number of ships to generate</param>
        /// <param name="gameConfiguration">Ship configuration</param>
        public ShipPositioningParametersGenerator(GameConfiguration gameConfiguration)
        {
            _gameConfiguration = gameConfiguration;
        }

        public IEnumerable<IEnumerable<ShipPositioningParameters>> GenerateRandomInputSequences()
        {
            var random = new Random();

            var possiblePositionsOnBoard = BoardCoordinate.GetAllPossibleCoordinates(_gameConfiguration.BoardSize, _gameConfiguration.BoardSize).ToArray();

            // just for fun, let's drop all coordinates for a random letter
            possiblePositionsOnBoard =
                possiblePositionsOnBoard.Where(p => p.Row != random.Next()%9 && p.Column != random.Next()%9).ToArray();

            while (true)
            {
                var randomizedShips = _gameConfiguration.ShipLengths.Shuffle(new Random(random.Next())).ToArray();
                var singleBoardPrototype = new List<ShipPositioningParameters>(_gameConfiguration.ShipCount);

                for (int i = 0; i < _gameConfiguration.ShipCount; i++)
                {
                    var randomBoardCoordinate = possiblePositionsOnBoard[random.Next() % possiblePositionsOnBoard.Count()];
                    var randomDirection = (randomizedShips[i] > 1) ? random.Next() % 2 == 0 ? ShipDirection.Horizontal : ShipDirection.Vertical : ShipDirection.Horizontal;
                    singleBoardPrototype.Add(new ShipPositioningParameters { ShipCoordinate = randomBoardCoordinate, ShipDirection = randomDirection });
                }

                yield return singleBoardPrototype;
            }
        }
    }
}
