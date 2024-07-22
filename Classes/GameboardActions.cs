using Chess.Classes.ConcretePieces;
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


        /// <summary>
        /// Returns a list of pieces on the provided Gameboard that are currently checking the checkedTeamColour King
        /// </summary>
        public static List<Piece> GetCheckingPieces(this Gameboard gameboard, TeamColour checkedTeamColour)
        {
            var checkingPieces = new List<Piece>();
            var king = ChessUtils.FindKing(checkedTeamColour, gameboard.Board);
            var enemyActions = gameboard.GetAllPossibleActions(checkedTeamColour.GetOppositeTeam());

            // this should only happen in test situations where a King isn't placed on the board 
            if (king is null)
                return checkingPieces;

            foreach (var action in enemyActions)
            {
                // skip actions by pieces that have already been determined to check the King
                if (checkingPieces.Contains(action.Piece))
                    continue;

                // if the King is "capturable" by a Piece, it is checking the King
                if (action.Square == king.Square && action.ActionType == ActionType.Capture)
                    checkingPieces.Add(action.Piece);
            }

            return checkingPieces;
        }

        /// <summary>
        /// Returns a list of all possible Actions that pieces of the provided TeamColour can perform, WITHOUT considering a checked King
        /// </summary>
        public static List<Action> GetAllPossibleActions(this Gameboard gameboard, TeamColour teamColour)
        {
            var actions = new List<Action>();

            foreach (var piece in gameboard.Board)
            {
                if (piece == null) continue;
                if (piece.TeamColour != teamColour) continue;

                if (piece is Pawn)
                    actions.AddRange(piece.GetPotentialActions(gameboard.Board, gameboard.LastPerformedAction));
                else
                    actions.AddRange(piece.GetPotentialActions(gameboard.Board, null));
            }

            return actions;
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
