using Chess.Classes;
using Chess.Types;

namespace Chess
{
    internal class Program
    {

        static void Main(string[] args)
        {
            var gameboard = new Gameboard();
            gameboard.InitialiseBoardState();
            gameboard.DrawCurrentState();
            gameboard.CalculateTeamActions(TeamColour.Black);
        }
    }
}
