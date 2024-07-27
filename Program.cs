using Chess.Classes;

namespace Chess
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var gameboard = new Gameboard();
            gameboard.InitialiseTestBoardState();

            while (true)
            {
                gameboard.DrawCurrentState();
                var actions = gameboard.CalculateTeamActions(gameboard.CurrentTeamColour);
                var selectedAction = gameboard.SelectAction(actions);
                gameboard.AddActionToHistory(new Action(selectedAction));
                gameboard.PerformAction(selectedAction);

                Console.WriteLine($"White points: {gameboard.WhitePoints}");
                Console.WriteLine($"Black points: {gameboard.BlackPoints}");
            }
        }
    }
}
