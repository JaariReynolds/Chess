using Chess.Types;

namespace Chess.Classes
{
    public static class ChessUtils
    {
        private static bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        public static bool IsEmptySquare(int x, int y, Piece[,] boardState)
        {
            return IsWithinBounds(x, y) && boardState[x, y] == null;
        }

        public static bool IsNotNull(int x, int y, Piece[,] boardState)
        {
            return IsWithinBounds(x, y) && boardState[x, y] != null;
        }

        public static bool IsEnemy(TeamColour teamColour, int x, int y, Piece[,] boardState)
        {
            return IsNotNull(x, y, boardState) && boardState[x, y].TeamColour != teamColour;
        }

        public static bool EnPassant(TeamColour currentTeamColour, int x, int y, Action? lastPerformedAction)
        {
            if (lastPerformedAction == null || lastPerformedAction.ActionType != ActionType.PawnDoubleMove)
                return false;

            bool enPassant = false;

            switch (currentTeamColour)
            {
                // White en passant can only be performed on row index 2
                case TeamColour.White:
                    if (x == 2 && y == lastPerformedAction.ActionY)
                        enPassant = true;
                    break;

                // Black en passant can only be performed on row index 5
                case TeamColour.Black:
                    if (x == 5 && y == lastPerformedAction.ActionY)
                        enPassant = true;
                    break;
            }

            return enPassant;
        }


    }
}
