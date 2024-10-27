using Chess.Classes;
using Chess.Types;

namespace Chess
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var gameboard = new Gameboard();
            gameboard.InitialiseStandardBoardState();

            while (true)
            {
                var actions = gameboard.CalculateTeamActions(gameboard.CurrentTeamColour);

                if (actions.Count == 0) break;

                gameboard.DrawCurrentState();
                var selectedAction = gameboard.SelectAction(actions);
                gameboard.AddActionToHistory(new Action(selectedAction));
                gameboard.PerformAction(selectedAction);
            }

            Console.WriteLine($"{gameboard.CurrentTeamColour.GetOppositeTeam()} wins!");
            Console.WriteLine($"White points: {gameboard.WhitePoints}");
            Console.WriteLine($"Black points: {gameboard.BlackPoints}");
        }
    }
}
