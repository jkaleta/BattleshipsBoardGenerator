using System;

namespace BattleshipGameboardGenerator
{
    class Program
    {
        static void Main()
        {
            StartGame();
        }

        private static async void StartGame()
        {
            Logger.Enabled = false;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //var megaConfiguration = new GameConfiguration("Whacky Configuration", new[] { 40, 30, 30, 20, 20, 20, 10, 10, 10, 10, 
            //    5, 5, 6, 3, 2, 2, 2, 10, 7, 8, 40, 30, 30, 20, 20, 20, 10, 10, 10, 10, 5, 5, 6, 3, 2, 2, 
            //    20, 20, 10, 10, 10, 10, 5, 5, 6, 3, 2, 2, 2, 10, 7, 8, 2, 10, 7, 8 }, 50);

            //var strategy1 = new BruteForceBoardGenerationStrategy(GameConfiguration.MiltonBradleyConfiguration, 1000000);
            var strategy2 = new BruteForceBoardGenerationStrategy(GameConfiguration.RussianConfiguration, 1000000);

            //strategy1.GenerateBoards();
            strategy2.GenerateBoards();
            Console.ReadLine();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Log("Error occurred: " + e.ExceptionObject);
        }
    }


}
