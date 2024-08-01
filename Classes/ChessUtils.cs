using Chess.Types;

namespace Chess.Classes
{
    public static class ChessUtils
    {
        public static bool IsWithinBounds(Square square)
        {
            return square.X >= 0 && square.X < 8 && square.Y >= 0 && square.Y < 8;
        }

        public static bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        public static bool IsNotNull(Square square, Piece[][] boardState)
        {
            return IsWithinBounds(square) && boardState[square.X][square.Y] != null;
        }

        public static bool IsEmptySquare(Square square, Piece[][] boardState)
        {
            return IsWithinBounds(square) && boardState[square.X][square.Y] == null;
        }

        public static bool IsEnemy(TeamColour teamColour, Square square, Piece[][] boardState)
        {
            return IsNotNull(square, boardState) && boardState[square.X][square.Y].TeamColour != teamColour;
        }

        public static bool EnPassant(TeamColour currentTeamColour, Square square, Action? lastPerformedAction)
        {
            if (lastPerformedAction == null || lastPerformedAction.ActionType != ActionType.PawnDoubleMove)
                return false;

            bool enPassant = false;

            switch (currentTeamColour)
            {
                // White en passant can only be performed on row index 2
                case TeamColour.White:
                    if (square.X == 2 && square.Y == lastPerformedAction.Square.Y)
                        enPassant = true;
                    break;

                // Black en passant can only be performed on row index 5
                case TeamColour.Black:
                    if (square.X == 5 && square.Y == lastPerformedAction.Square.Y)
                        enPassant = true;
                    break;
            }

            return enPassant;
        }

        public static bool DeterminePieceAction(Piece piece, List<Action> actions, Square square, Piece[][] boardState)
        {
            // deadEnd used for Rook, Bishop, Queen, where their directional moves stop after an obstruction

            bool deadEnd = true;
            if (IsEmptySquare(square, boardState))
            {
                actions.Add(new Action(piece, square, ActionType.Move));
                deadEnd = false;
            }
            else if (IsEnemy(piece.TeamColour, square, boardState))
            {
                actions.Add(new Action(piece, square, ActionType.Capture));
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
        public static string ToAlgebraicNotation(Square square)
        {
            char file = (char)('a' + square.Y);
            int rank = 8 - square.X;
            return $"{file}{rank}";
        }

        public static Piece[][] InitialiseBoard()
        {
            var board = new Piece[8][];

            for (int i = 0; i < 8; i++)
                board[i] = new Piece[8];

            return board;
        }
    }
}
