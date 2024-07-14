using Chess.Types;

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


        public static void Move(this Gameboard gameboard, Action action, Piece[,] boardState)
        {
            var actionSquareNotation = action.Square.ToString();
            var originalSquareNotation = action.Piece.Square.ToString();
            var piece = action.Piece;

            boardState.SetPieceAt(actionSquareNotation, piece); // update piece position on the board
            boardState.SetPieceAt(originalSquareNotation, null); // set old square to null
            piece.MovePiece(action.Square); // actually move the piece
        }

        public static void Capture(this Gameboard gameboard, Action action, Piece[,] boardState)
        {
            // Capture functionally the same as a move, just with awarded points
            Piece capturedPiece = GetPieceAt(boardState, action.Square.ToString());
            if (capturedPiece.TeamColour == TeamColour.White)
                gameboard.BlackPoints += capturedPiece.PieceValue;
            else
                gameboard.WhitePoints += capturedPiece.PieceValue;

            Move(gameboard, action, boardState);

        }
    }
}
