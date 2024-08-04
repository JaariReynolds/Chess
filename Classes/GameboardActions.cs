using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace Chess.Classes
{
    public static class GameboardActions
    {
        /// <summary>
        /// Returns a Piece if found, otherwise returns null.
        /// </summary>

        public static Piece? GetPieceAt(this Piece[][] boardState, string algebraicNotation)
        {
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            return boardState[x][y];
        }

        /// <summary>
        /// Sets the provided square to null.
        /// </summary>
        public static void ClearSquare(this Piece[][] boardState, string algebraicNotation)
        {
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            boardState[x][y] = null!; // passing a null piece is legal, will just mean setting that coord to empty
        }

        /// <summary>
        /// Place the provided piece on the board according to the Piece's current Square coordinates.
        /// </summary>
        public static void SetSquare(this Piece[][] boardState, Piece piece)
        {
            boardState[piece.Square.X][piece.Square.Y] = piece;
        }

        public static Piece? FindKing(this Piece[][] boardState, TeamColour teamColour)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = boardState[i][j];
                    if (piece is King && piece.TeamColour == teamColour)
                        return piece;
                }
            }

            return null;
        }

        public static bool IsKingInCheck(this Piece[][] boardState, TeamColour teamColour, Action? previousAction)
        {
            var king = boardState.FindKing(teamColour);

            // this should only happen in test situations where a King isn't placed on the board 
            if (king is null)
                return false;

            var isKingUnderAttack = boardState.IsSquareUnderAttack(king.Square, teamColour.GetOppositeTeam(), previousAction);
            return isKingUnderAttack;
        }

        public static bool IsSquareUnderAttack(this Piece[][] boardState, Square square, TeamColour opponentTeamColour, Action? previousAction)
        {
            var opponentActions = boardState.GetAllPossibleActions(opponentTeamColour, previousAction);

            foreach (var action in opponentActions)
                if (action.Square == square && action.ActionType == ActionType.Capture)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns a list of pieces on the provided board that are currently checking the checkedTeamColour King.
        /// </summary>
        public static List<Piece> GetCheckingPieces(this Piece[][] boardState, TeamColour checkedTeamColour, Action? previousAction)
        {
            var checkingPieces = new List<Piece>();
            var king = boardState.FindKing(checkedTeamColour);
            var enemyActions = boardState.GetAllPossibleActions(checkedTeamColour.GetOppositeTeam(), previousAction);

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
        /// Returns a list of all possible Actions that pieces of the provided TeamColour can perform, including Actions that leave the King in a checked position.
        /// </summary>
        public static List<Action> GetAllPossibleActions(this Piece[][] boardState, TeamColour teamColour, Action? previousAction)
        {
            var actions = new List<Action>();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var piece = boardState[i][j];
                    if (piece == null) continue;
                    if (piece.TeamColour != teamColour) continue;

                    actions.AddRange(piece.GetPotentialActions(boardState, previousAction));
                }
            }

            return actions;
        }

        /// <summary>
        /// Returns a list of legal actions that do not leave the King in a checked position. 
        /// Requires the whole gameboard object in order to simulate performing potential actions.
        /// </summary>
        public static List<Action> GetLegalActions(this Gameboard gameboard, List<Action> possibleActions)
        {
            // legal actions are ones that do not leave the King in a checked position
            var legalActions = new List<Action>();
            var previousAction = gameboard.PreviousActions.Count == 0 ? null : gameboard.PreviousActions[^1];

            if (possibleActions.Count == 0)
                return legalActions;

            // foreach possible action, simulate performing the action and recalculate the enemy actions to see if checkingPieces == 0
            foreach (var action in possibleActions)
            {
                var simulatedBoard = new Gameboard(gameboard);
                var simulatedAction = new Action(action);
                simulatedBoard.PerformAction(simulatedAction);

                // if no checking pieces after the simulated action, means it counts as a legal action
                if (GetCheckingPieces(simulatedBoard.Board, action.Piece.TeamColour, previousAction).Count == 0)
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
            var capturedPiece = GetPieceAt(gameboard.Board, action.Square.ToString());
            if (capturedPiece!.TeamColour == TeamColour.White)
                gameboard.BlackPoints += capturedPiece.PieceValue;
            else
                gameboard.WhitePoints += capturedPiece.PieceValue;

            Move(gameboard, action);
        }

        public static void Promote(this Gameboard gameboard, Action action)
        {
            // create a new piece at the promoted square
            switch (action.ActionType)
            {
                case ActionType.PawnPromoteKnight:
                    gameboard.Board.SetSquare(new Knight(action.Piece.TeamColour, action.Square.ToString()));
                    break;
                case ActionType.PawnPromoteBishop:
                    gameboard.Board.SetSquare(new Bishop(action.Piece.TeamColour, action.Square.ToString()));
                    break;
                case ActionType.PawnPromoteRook:
                    gameboard.Board.SetSquare(new Rook(action.Piece.TeamColour, action.Square.ToString()));
                    break;
                case ActionType.PawnPromoteQueen:
                    gameboard.Board.SetSquare(new Queen(action.Piece.TeamColour, action.Square.ToString()));
                    break;
                default:
                    throw new ArgumentException($"Promote method should not be called with a the non-promotion action: {action.ActionType}");
            }

            gameboard.Board.ClearSquare(action.Piece.Square.ToString()); // clear the original square of the pawn 
        }

        public static void EnPassant(this Gameboard gameboard, Action action)
        {
            // "capture" the Pawn that double moved (i.e. the piece that performed the last action)
            var pawnPieceAction = gameboard.PreviousActions[^1];

            gameboard.Board.ClearSquare(pawnPieceAction.Square.ToString()); // remove the pawn that double moved 

            // award points (1) of capture to the current team
            if (action.Piece.TeamColour == TeamColour.White)
                gameboard.WhitePoints += pawnPieceAction.Piece.PieceValue;
            else
                gameboard.BlackPoints += pawnPieceAction.Piece.PieceValue;

            // move capturing piece to standard capture square (1 diagonal)
            Move(gameboard, action);
        }

        public static void KingsideCastle(this Gameboard gameboard, Action action)
        {
            // King moves to g1/8, Rook moves to f1/8
            var rank = action.Piece.TeamColour == TeamColour.White ? "1" : "8";
            var kingsideRook = GetPieceAt(gameboard.Board, $"h{rank}");

            gameboard.Move(new Action(action.Piece, $"g{rank}", ActionType.Move));
            gameboard.Move(new Action(kingsideRook!, $"f{rank}", ActionType.Move));

            // Castles are the only actions where more than 1 piece is moved.
            // Update rook.HasMoved here as it won't happen in Gameboard.PerformAction()
            kingsideRook!.HasMoved = true;
        }

        public static void QueensideCastle(this Gameboard gameboard, Action action)
        {
            // King moves to c1/8, Rook moves to d1/8
            var rank = action.Piece.TeamColour == TeamColour.White ? "1" : "8";
            var queensideRook = GetPieceAt(gameboard.Board, $"a{rank}");

            gameboard.Move(new Action(action.Piece, $"c{rank}", ActionType.Move));
            gameboard.Move(new Action(queensideRook!, $"d{rank}", ActionType.Move));

            // Castles are the only actions where more than 1 piece is moved.
            // Update rook.HasMoved here as it won't happen in Gameboard.PerformAction()
            queensideRook!.HasMoved = true;
        }
    }
}
