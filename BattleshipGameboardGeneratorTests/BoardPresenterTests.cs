using System;
using BattleshipGameboardGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipGameboardGeneratorTests
{
    [TestClass]
    public class BoardPresenterTests
    {
        [TestMethod]
        public void PresentBoardAsString()
        {
            // Arrange
            var board = new Board(new GameConfiguration("", new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 }, 10));
            board.BoardRepresentation.Add(new BoardCoordinate(1, 0));
            board.BoardRepresentation.Add(new BoardCoordinate(2, 0));
            board.BoardRepresentation.Add(new BoardCoordinate(3, 0));
            board.BoardRepresentation.Add(new BoardCoordinate(4, 0));
            board.BoardRepresentation.Add(new BoardCoordinate(3, 3));
            board.BoardRepresentation.Add(new BoardCoordinate(3, 4));
            board.BoardRepresentation.Add(new BoardCoordinate(3, 5));
            board.BoardRepresentation.Add(new BoardCoordinate(5, 5));
            board.BoardRepresentation.Add(new BoardCoordinate(5, 6));
            board.BoardRepresentation.Add(new BoardCoordinate(5, 7));
            board.BoardRepresentation.Add(new BoardCoordinate(6, 0));
            board.BoardRepresentation.Add(new BoardCoordinate(6, 1));
            board.BoardRepresentation.Add(new BoardCoordinate(7, 3));
            board.BoardRepresentation.Add(new BoardCoordinate(7, 4));
            board.BoardRepresentation.Add(new BoardCoordinate(9, 0));
            board.BoardRepresentation.Add(new BoardCoordinate(9, 1));
            board.BoardRepresentation.Add(new BoardCoordinate(9, 7));
            board.BoardRepresentation.Add(new BoardCoordinate(9, 9));
            board.BoardRepresentation.Add(new BoardCoordinate(0, 4));
            board.BoardRepresentation.Add(new BoardCoordinate(0, 6));

            var expected =
                "0000101000" + Environment.NewLine +
                "1000000000" + Environment.NewLine +
                "1000000000" + Environment.NewLine +

                "1001110000" + Environment.NewLine +
                "1000000000" + Environment.NewLine +
                "0000011100" + Environment.NewLine +

                "1100000000" + Environment.NewLine +
                "0001100000" + Environment.NewLine +
                "0000000000" + Environment.NewLine +

                "1100000101" + Environment.NewLine;

            // Act
            var representation = board.PresentBoardGraphically('0', '1');

            // Assert
            Assert.AreEqual(expected, representation);
        }
    }
}
