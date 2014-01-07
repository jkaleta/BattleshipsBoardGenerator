using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BattleshipGameboardGenerator
{
    public class BruteForceBoardGenerationStrategy
    {
        private readonly ShipConfiguration _shipConfiguration;

        public BruteForceBoardGenerationStrategy(ShipConfiguration shipConfiguration)
        {
            _shipConfiguration = shipConfiguration;
        }

        private readonly Queue<IEnumerable<ShipPositioningParameters>> _mainQueue =
            new Queue<IEnumerable<ShipPositioningParameters>>();

        private readonly Queue<Board> _boardQueue =
            new Queue<Board>();

        private void PopulateProcessingQueue()
        {
            Task.Factory.StartNew(() =>
                {
                    var allPossiblePositions = _shipConfiguration.GetListOfPossibleCombinationsOfShipPositions();

                    foreach (var list in allPossiblePositions)
                    {

                        // if the list has duplicates, skip it
                        if(list.Count() != list.Distinct().Count())
                            continue;

                        _mainQueue.Enqueue(list);

                        if (_mainQueue.Count > 1000)
                            Thread.Sleep(5000);
                    }
                });
        }

        public void GenerateBoards()
        {
            Action produceBoards = () =>
                {
                    var exceptionCount = 0;

                    while (exceptionCount < 5)
                    {
                        try
                        {
                            if (_mainQueue.Count == 0)
                                Thread.Sleep(5000);

                            var config = _mainQueue.Dequeue().ToArray();
                            var board = new SingleBoardGenerator(_shipConfiguration).GenerateBoard(config);
                            _boardQueue.Enqueue(board);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                };

            Action writeToFile = () =>
                {
                    while (true)
                    {
                        try
                        {
                            if (_boardQueue.Count < 1000)
                            {
                                Thread.Sleep(5000);
                                continue;
                            }

                            var toWriteToFile = new List<Board>(1000);
                            for (int i = 0; i < 1000; i++)
                                toWriteToFile.Add(_boardQueue.Dequeue());

                            using (var file = new System.IO.StreamWriter(@"Boards.txt"))
                            {
                                foreach (var board in toWriteToFile)
                                {
                                    file.WriteLine(board);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            break;
                        }
                    }
                };

            PopulateProcessingQueue();
            Task.Factory.StartNew(produceBoards);
            //Task.Factory.StartNew(produceBoards);
            //Task.Factory.StartNew(produceBoards);
            Task.Factory.StartNew(writeToFile);
        }
    }



    internal enum CellStatus
    {
        Free,
        ShipPart,
        NotAvailableForPlacement
    }
}
