using System;
using BattleshipGameboardGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BattleshipGameboardGeneratorTests
{
    [TestClass]
    public class BoardCoordinateTests
    {
        [TestMethod]
        public void ToString_ShouldPrintHumanReadableCoordinates()
        {
            Assert.AreEqual("A1", new BoardCoordinate(0, 0).ToString());
            Assert.AreEqual("B2", new BoardCoordinate(1, 1).ToString());
            Assert.AreEqual("C3", new BoardCoordinate(2, 2).ToString());
            Assert.AreEqual("D4", new BoardCoordinate(3, 3).ToString());
            Assert.AreEqual("E5", new BoardCoordinate(4, 4).ToString());
            Assert.AreEqual("F6", new BoardCoordinate(5, 5).ToString());
            Assert.AreEqual("G7", new BoardCoordinate(6, 6).ToString());
            Assert.AreEqual("H8", new BoardCoordinate(7, 7).ToString());
            Assert.AreEqual("I9", new BoardCoordinate(8, 8).ToString());
            Assert.AreEqual("J10", new BoardCoordinate(9, 9).ToString());
        }
    }
}
