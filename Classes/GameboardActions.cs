namespace Chess.Classes
{
    public static class GameboardActions
    {
        public static Piece GetPieceAt(this Piece[,] boardState, string algebraicNotation)
        {
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            return boardState[x, y];
        }

        public static void SetPieceAt(this Piece[,] boardState, string algebraicNotation, Piece? piece)
        {
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            boardState[x, y] = piece; // passing a null piece is legal, will just mean setting that coord to empty
        }


        public static void Move(Action action, Piece[,] boardState)
        {
            if (action.ActionType != ActionType.Move)
                throw new InvalidOperationException($"'{action}' action should not be called from the Move() method.");

            var actionSquareNotation = action.Square.ToString();
            var originalSquareNotation = action.Piece.Square.ToString();
            var piece = action.Piece;

            boardState.SetPieceAt(actionSquareNotation, piece); // update piece position on the board
            boardState.SetPieceAt(originalSquareNotation, null); // set old square to null
            piece.MovePiece(action.Square); // actually move the piece
        }
    }
}
