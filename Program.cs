using Chess.Classes;

namespace Chess
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var gameboard = new Gameboard();
            gameboard.InitialiseBoardState();

            while (true)
            {
                gameboard.DrawCurrentState();
                var actions = gameboard.CalculateTeamActions(gameboard.CurrentTeamColour);
                var selectedAction = gameboard.SelectAction(actions);
                gameboard.PerformAction(selectedAction);

                Console.WriteLine($"White points: {gameboard.WhitePoints}");
                Console.WriteLine($"Black points: {gameboard.BlackPoints}");
            }
        }
    }
}
