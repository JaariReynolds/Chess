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

        public static bool IsKingInCheck(this Gameboard gameboard, TeamColour teamColour)
        {
            // to check if teamColour's King is in check, only the opposite team's moves need to be calculated
            var enemyActions = gameboard.CalculateTeamActions(teamColour.GetOppositeTeam());
            var king = ChessUtils.FindKing(teamColour, gameboard.Board);

            // check if the King is now threatened by a capture from any of the available enemy moves
            foreach (var action in enemyActions)
                if (action.Square == king.Square && action.ActionType == ActionType.Capture)
                    return true;

            return false;
        }

        public static List<Piece> GetCheckingPieces(this Gameboard gameboard, TeamColour checkedTeamColour)
        {
            var checkingPieces = new List<Piece>();
            var enemyActions = gameboard.CalculateTeamActions(checkedTeamColour.GetOppositeTeam());
            var king = ChessUtils.FindKing(checkedTeamColour, gameboard.Board);

            // JUST COMBINE ISKINGINCHECK AND GETCHECKINGPIECES AS THEYRE DOING VERY SIMILAR ACTIONS AND WASTING RECALCULATION TIME

            foreach (var action in enemyActions)
            {
                if (checkingPieces.Contains(action.Piece))
                    continue;

                // if the King is "capturable" by a Piece, it is checking the King
                if (action.Square == king.Square && action.ActionType == ActionType.Capture)
                    checkingPieces.Add(action.Piece);
            }

            return checkingPieces;
        }

        public static void Move(this Gameboard gameboard, Action action)
        {
            var actionSquareNotation = action.Square.ToString();
            var originalSquareNotation = action.Piece.Square.ToString();
            var piece = action.Piece;

            gameboard.Board.SetPieceAt(actionSquareNotation, piece); // update piece position on the board
            gameboard.Board.SetPieceAt(originalSquareNotation, null); // set old square to null
            piece.MovePiece(action.Square); // actually move the piece
        }

        public static void Capture(this Gameboard gameboard, Action action)
        {
            // Capture functionally the same as a move, just with awarded points
            Piece capturedPiece = GetPieceAt(gameboard.Board, action.Square.ToString());
            if (capturedPiece.TeamColour == TeamColour.White)
                gameboard.BlackPoints += capturedPiece.PieceValue;
            else
                gameboard.WhitePoints += capturedPiece.PieceValue;

            Move(gameboard, action);
        }
    }
}
