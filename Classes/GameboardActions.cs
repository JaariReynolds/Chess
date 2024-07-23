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

        public static void ClearSquare(this Piece[,] boardState, string algebraicNotation)
        {
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            boardState[x, y] = null!; // passing a null piece is legal, will just mean setting that coord to empty
        }

        public static void SetSquare(this Piece[,] boardState, Piece piece)
        {
            boardState[piece.Square.X, piece.Square.Y] = piece;
        }

        public static Piece? FindKing(this Gameboard gameboard, TeamColour teamColour)
        {
            foreach (var piece in gameboard.Board)
                if (piece is King && piece.TeamColour == teamColour)
                    return piece;

            return null;
        }

        /// <summary>
        /// Returns a list of pieces on the provided Gameboard that are currently checking the checkedTeamColour King
        /// </summary>
        public static List<Piece> GetCheckingPieces(this Gameboard gameboard, TeamColour checkedTeamColour)
        {
            var checkingPieces = new List<Piece>();
            var king = gameboard.FindKing(checkedTeamColour);
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
        /// Returns a list of all possible Actions that pieces of the provided TeamColour can perform, including Actions that leave the King in a checked position
        /// </summary>
        public static List<Action> GetAllPossibleActions(this Gameboard gameboard, TeamColour teamColour)
        {
            var actions = new List<Action>();

            foreach (var piece in gameboard.Board)
            {
                if (piece == null) continue;
                if (piece.TeamColour != teamColour) continue;

                if (piece is Pawn)
                    actions.AddRange(piece.GetPotentialActions(gameboard.Board, gameboard.PreviousActions.Count > 0 ? gameboard.PreviousActions[^1] : null));
                else
                    actions.AddRange(piece.GetPotentialActions(gameboard.Board, null));
            }

            return actions;
        }

        /// <summary>
        /// Returns a list of legal actions that do not leave the King in a checked position
        /// </summary>
        public static List<Action> GetLegalActions(this Gameboard gameboard, List<Action> possibleActions)
        {
            // legal actions are ones that do not leave the King in a checked position
            var legalActions = new List<Action>();

            if (possibleActions.Count == 0)
                return legalActions;

            // foreach possible action, simulate performing the action and recalculate the enemy actions to see if checkingPieces == 0
            foreach (var action in possibleActions)
            {
                var simulatedBoard = new Gameboard(gameboard);
                var simulatedAction = new Action(action);
                simulatedBoard.PerformAction(simulatedAction);

                // if no checking pieces after the simulated action, means it counts as a legal action
                if (GetCheckingPieces(simulatedBoard, action.Piece.TeamColour).Count == 0)
                    legalActions.Add(action);
            }

            return legalActions;
        }

        public static void Move(this Gameboard gameboard, Action action)
        {
            var originalSquareNotation = action.Piece.Square.ToString();
            var piece = action.Piece;

            piece.MovePiece(action.Square); // actually move the piece
            gameboard.Board.SetSquare(piece); // update piece position on the board
            gameboard.Board.ClearSquare(originalSquareNotation); // set old square to null
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
