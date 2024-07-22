using Chess.Types;

namespace Chess.Classes
{
    public static class ChessUtils
    {
        private static bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        private static bool IsNotNull(int x, int y, Piece[,] boardState)
        {
            return IsWithinBounds(x, y) && boardState[x, y] != null;
        }

        public static bool IsEmptySquare(int x, int y, Piece[,] boardState)
        {
            return IsWithinBounds(x, y) && boardState[x, y] == null;
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
                    if (x == 2 && y == lastPerformedAction.Square.Y)
                        enPassant = true;
                    break;

                // Black en passant can only be performed on row index 5
                case TeamColour.Black:
                    if (x == 5 && y == lastPerformedAction.Square.Y)
                        enPassant = true;
                    break;
            }

            return enPassant;
        }

        public static bool DeterminePieceAction(Piece piece, List<Action> actions, int x, int y, Piece[,] boardState)
        {
            // deadEnd used for Rook, Bishop, Queen, where their directional moves stop after an obstruction

            bool deadEnd = true;
            if (IsEmptySquare(x, y, boardState))
            {
                actions.Add(new Action(piece, x, y, ActionType.Move));
                deadEnd = false;
            }
            else if (IsEnemy(piece.TeamColour, x, y, boardState))
            {
                actions.Add(new Action(piece, x, y, ActionType.Capture));
            }

            return deadEnd;
        }

        public static (int, int) CoordsFromAlgebraicNotation(string algebraicNotation)
        {
            if (algebraicNotation.Length != 2)
                throw new ArgumentException("Algebraic notation must be exactly 2 characters.");

            char fileChar = algebraicNotation[0];
            char rankChar = algebraicNotation[1];

            if (fileChar < 'a' || fileChar > 'h' || rankChar < '1' || rankChar > '8')
                throw new ArgumentException($"{algebraicNotation} is not a valid square");

            int column = fileChar - 'a';
            int row = '8' - rankChar;
            return (row, column);
        }

        // Converts array indexes to standard algebraic notation used in Chess (a8 top left, h1 bottom right)
        public static string ToAlgebraicNotation(int x, int y)
        {
            char file = (char)('a' + y);
            int rank = 8 - x;
            return $"{file}{rank}";
        }
    }
}
