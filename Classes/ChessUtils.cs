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

        public static bool IsEnemy(TeamColour friendlyTeamColour, Square square, Piece[][] boardState)
        {
            return IsNotNull(square, boardState) && boardState[square.X][square.Y].TeamColour != friendlyTeamColour;
        }

        public static bool IsPawnPromoteAction(Action action)
        {
            return
                action.ActionType == ActionType.PawnPromoteKnight ||
                action.ActionType == ActionType.PawnPromoteBishop ||
                action.ActionType == ActionType.PawnPromoteRook ||
                action.ActionType == ActionType.PawnPromoteQueen;
        }

        public static bool CanEnPassant(TeamColour currentTeamColour, Square square, Action? lastPerformedAction)
        {
            bool canEnPassant = false;

            // en passant captures only valid when the last performed move is a pawn double move
            if (lastPerformedAction == null || lastPerformedAction.ActionType != ActionType.PawnDoubleMove)
                return canEnPassant;

            switch (currentTeamColour)
            {
                // White en passant can only be performed on row index 2
                case TeamColour.White:
                    if (square.X == 2 && square.Y == lastPerformedAction.Square.Y)
                        canEnPassant = true;
                    break;

                // Black en passant can only be performed on row index 5
                case TeamColour.Black:
                    if (square.X == 5 && square.Y == lastPerformedAction.Square.Y)
                        canEnPassant = true;
                    break;
            }

            return canEnPassant;
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
