using Chess.Classes;

namespace Chess
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var gameboard = new Gameboard();
            gameboard.InitialiseBoardState();
            gameboard.IgnoreKing = true;

            while (true)
            {
                gameboard.DrawCurrentState();
                var teamColour = gameboard.CurrentTeamColour;
                var actions = gameboard.CalculateTeamActions(teamColour);
                var selectedAction = gameboard.SelectAction(actions);
                gameboard.PerformAction(selectedAction);

                Console.WriteLine($"White points: {gameboard.WhitePoints}");
                Console.WriteLine($"Black points: {gameboard.BlackPoints}");
            }
        }
    }
}
