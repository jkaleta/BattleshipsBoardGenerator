using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BattleshipGameboardGenerator
{
    public class BruteForceBoardGenerationStrategy
    {
        private readonly GameConfiguration _gameConfiguration;
        private readonly int _maxShipsToGenerate;
        private readonly int _pageSize;
        private readonly object _locker = new Object();
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public BruteForceBoardGenerationStrategy(GameConfiguration gameConfiguration, int maxShipsToGenerate)
        {
            _gameConfiguration = gameConfiguration;
            _maxShipsToGenerate = maxShipsToGenerate;
            _pageSize = maxShipsToGenerate / 10;
        }

        private readonly Queue<IEnumerable<ShipPositioningParameters>> _mainQueue =
            new Queue<IEnumerable<ShipPositioningParameters>>();

        private readonly Queue<Board> _boardQueue =
            new Queue<Board>();

        private void PopulateProcessingQueue(CancellationToken cancellationToken)
        {
            Logger.Log("Starting Input Sequence Generating Thread {0}", Thread.CurrentThread.ManagedThreadId);

            while (!cancellationToken.IsCancellationRequested)
            {
                var inputSequences = new ShipPositioningParametersGenerator(_gameConfiguration).GenerateRandomInputSequences().Take(5000);
                Logger.Log("Input Sequence Generating Thread {0} generated 5000 sequences.", Thread.CurrentThread.ManagedThreadId);

                foreach (var list in inputSequences)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var elements = list.ToArray();
                    // if the list has duplicates, skip it
                    if (elements.Count() != elements.Distinct().Count())
                        continue;

                    lock (_locker)
                    {
                        _mainQueue.Enqueue(elements);
                    }

                    if (_mainQueue.Count > _pageSize)
                    {
                        Thread.Sleep(500);
                    }
                }
            }

            Logger.Log("Terminating Input Sequence Generating Thread {0}", Thread.CurrentThread.ManagedThreadId);
        }

        private void ProduceBoards(CancellationToken cancellationToken)
        {
            Logger.Log("Starting Board Producing Thread {0}", Thread.CurrentThread.ManagedThreadId);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_mainQueue.Count == 0)
                {
                    Thread.Sleep(20);
                    continue;
                }

                ShipPositioningParameters[] config;
                lock (_locker)
                {
                    if (_mainQueue.Count == 0)
                        continue;

                    config = _mainQueue.Dequeue().ToArray();
                }

                Logger.Log("Board Producing Thread {0} starting to produce board", Thread.CurrentThread.ManagedThreadId);
                var board = new SingleBoardGenerator(_gameConfiguration).GenerateBoard(config);
                Logger.Log("Board Producing Thread {0} just produced new board: {1}", Thread.CurrentThread.ManagedThreadId, board.PresentBoardGraphically());

                _boardQueue.Enqueue(board);
            }

            Logger.Log("Terminating Board Producing Thread {0}", Thread.CurrentThread.ManagedThreadId);
        }

        public void WriteToFile(CancellationToken cancellationToken)
        {
            Logger.Log("Starting File Writing Thread {0}", Thread.CurrentThread.ManagedThreadId);

            CleanOutput();

            int counter = 1;
            var writtenToFileCount = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var fileName = GetFileName(counter);

                try
                {
                    if (_boardQueue.Count < _pageSize)
                    {
                        Thread.Sleep(2000);
                        continue;
                    }

                    var toWriteToFile = new List<string>(_pageSize);
                    for (int i = 0; i < _pageSize; i++)
                    {
                        var board = _boardQueue.Dequeue();
                        if (board == null)
                            continue;

                        toWriteToFile.Add(board.ToString());
                        //toWriteToFile.Add(Environment.NewLine + board.PresentBoardGraphically() + Environment.NewLine);
                    }

                    var distinctLines = toWriteToFile.Distinct().Shuffle(new Random()).ToArray();
                    writtenToFileCount += toWriteToFile.Count;

                    File.AppendAllLines(fileName, distinctLines);
                    Logger.Log("File Writing Thread writing {0} lines to file {1}", distinctLines.Count(), fileName);

                    if (new FileInfo(fileName).Length > 25 * 1024 * 1024)
                    {
                        counter++;
                    }

                    if (writtenToFileCount >= _maxShipsToGenerate)
                    {
                        _cts.Cancel();
                        Logger.Log("Generated {0} boards, press a key to finish.", _maxShipsToGenerate);
                    }
                }
                catch
                {
                    _cts.Cancel();
                }
            }
        }

        private void CleanOutput()
        {
            // shut up, I know I could have used RegEx. #Lazy
            for (int i = 0; i < 100; i++)
            {
                var fileName = GetFileName(i);
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
        }

        private string GetFileName(int counter)
        {
            var fileName = @"Boards for " + _gameConfiguration.FriendlyName + "_" + counter + ".txt";
            return fileName;
        }

        public void GenerateBoards()
        {
            var token = _cts.Token;

            Task.Factory.StartNew(() => PopulateProcessingQueue(token));
            Task.Factory.StartNew(() => PopulateProcessingQueue(token));
            Task.Factory.StartNew(() => PopulateProcessingQueue(token));
            Task.Factory.StartNew(() => ProduceBoards(token));
            Task.Factory.StartNew(() => ProduceBoards(token));
            Task.Factory.StartNew(() => ProduceBoards(token));
            Task.Factory.StartNew(() => WriteToFile(token));
        }
    }



    internal enum CellStatus
    {
        Free,
        ShipPart,
        NotAvailableForPlacement
    }
}
