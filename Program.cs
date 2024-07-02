using Chess.Classes;

namespace Chess
{
    internal class Program
    {
        private enum GameplayState
        {
            SETUP,
            AWAITING_INPUT,
            PERFORM_ACTION,
        }

        static void Main(string[] args)
        {
            var gameboard = new Gameboard();
            gameboard.InitialiseBoardState();
            gameboard.DrawCurrentState();
            gameboard.SwapTurns();
            gameboard.CalculateCurrentTeamActions();
            gameboard.SelectPiece();






        }
    }
}

/*
 * Square <- abstract Piece <- concrete specific Piece (i.e. Pawn, Queen)
 * 
 * Gameboard 
 *  - gameboard[,]
 *  - currentTurn (team)
 *  - currentTurnAvailableMoves
 * 
 * 
 * 
 * 
 * 
 * 
 */
