using Chess.Types;
using ChessLogic.Classes;

namespace Chess.Classes.ConcretePieces
{
    public class Pawn : Piece
    {
        private int startX;
        private int lastRow;
        private int actionAmount;

        internal Pawn() : base() { }

        public Pawn(TeamColour teamColour, string algebraicNotation, bool hasMoved) : base(teamColour, algebraicNotation, hasMoved, PieceName.Pawn, 1)
        {
            startX = teamColour == TeamColour.White ? 6 : 1;
            lastRow = teamColour == TeamColour.White ? 0 : 7;
            actionAmount = teamColour == TeamColour.White ? -1 : 1;
        }

        public Pawn(TeamColour teamColour, string algebraicNotation) : this(teamColour, algebraicNotation, false) { }

        public Pawn(TeamColour teamColour, int x, int y) : this(teamColour, AlgebraicNotationUtils.ToAlgebraicNotation(new Square(x, y))) { }

        public override Piece Clone()
        {
            return new Pawn(TeamColour, Square.ToString(), HasMoved);
        }

        public override List<Action> GetPotentialActions(Piece[][] boardState, bool includeCastles, Action? previousAction)
        {
            var actions = new List<Action>();

            Square? oneSquareForward = ChessUtils.IsWithinBounds(Square.X + actionAmount, Square.Y)
                ? new Square(Square.X + actionAmount, Square.Y)
                : null;

            Square? twoSquaresForward = ChessUtils.IsWithinBounds(Square.X + (actionAmount * 2), Square.Y)
                ? new Square(Square.X + (actionAmount * 2), Square.Y)
                : null;

            Square? diagonalLeft = ChessUtils.IsWithinBounds(Square.X + actionAmount, Square.Y - 1)
               ? new Square(Square.X + actionAmount, Square.Y - 1)
               : null;

            Square? diagonalRight = ChessUtils.IsWithinBounds(Square.X + actionAmount, Square.Y + 1)
                ? new Square(Square.X + actionAmount, Square.Y + 1)
                : null;

            AddMoves(actions, boardState, oneSquareForward, twoSquaresForward);
            AddStandardCaptures(actions, boardState, diagonalLeft, diagonalRight);
            AddEnPassantCaptures(actions, diagonalLeft, diagonalRight, previousAction);

            return actions;
        }

        private void AddPromoteActions(List<Action> actions, Square square, int? capturePoints)
        {
            actions.Add(new Action(this, square, ActionType.PawnPromoteKnight, capturePoints));
            actions.Add(new Action(this, square, ActionType.PawnPromoteBishop, capturePoints));
            actions.Add(new Action(this, square, ActionType.PawnPromoteRook, capturePoints));
            actions.Add(new Action(this, square, ActionType.PawnPromoteQueen, capturePoints));
        }

        private void AddMoves(List<Action> actions, Piece[][] boardState, Square? oneSquareForward, Square? twoSquaresForward)
        {
            // can move 1 at any point, but not capture
            if (oneSquareForward != null &&
                ChessUtils.IsEmptySquare(oneSquareForward, boardState))
            {
                if (Square.X + actionAmount == lastRow) // promote if moving to the last row
                    AddPromoteActions(actions, oneSquareForward, null);
                else
                    actions.Add(new Action(this, oneSquareForward, ActionType.Move));
            }

            // can move 2 squares on the first move, if no obstruction
            if (oneSquareForward != null &&
                twoSquaresForward != null &&
                Square.X == startX &&
                !HasMoved &&
                ChessUtils.IsEmptySquare(oneSquareForward, boardState) &&
                ChessUtils.IsEmptySquare(twoSquaresForward, boardState))
            {
                actions.Add(new Action(this, twoSquaresForward, ActionType.PawnDoubleMove));
            }
        }

        private void AddStandardCaptures(List<Action> actions, Piece[][] boardState, Square? diagonalLeft, Square? diagonalRight)
        {
            // Since a Pawn can capture + promote on the same action, promote takes priority and capture points are added to the Action as an optional

            // can capture 1 diagonally left
            if (diagonalLeft != null &&
                ChessUtils.IsEnemy(TeamColour, diagonalLeft, boardState))
            {
                if (Square.X + actionAmount == lastRow) // promote if capturing on the last row, otherwise capture
                {
                    var capturePromotePiece = boardState.GetPieceAt(diagonalLeft.ToString()); // non-null as IsEnemy confirms an enemy piece is on this square
                    AddPromoteActions(actions, diagonalLeft, capturePromotePiece!.PieceValue); // points added to action - only Pawn promote captures require this functionality 
                }
                else
                {
                    actions.Add(new Action(this, diagonalLeft, ActionType.Capture));
                }
            }

            // can capture 1 diagonally right
            if (diagonalRight != null &&
                ChessUtils.IsEnemy(TeamColour, diagonalRight, boardState))
            {
                if (Square.X + actionAmount == lastRow)
                {
                    var capturePromotePiece = boardState.GetPieceAt(diagonalRight.ToString());
                    AddPromoteActions(actions, diagonalRight, capturePromotePiece!.PieceValue);
                }
                else
                {
                    actions.Add(new Action(this, diagonalRight, ActionType.Capture));
                }
            }
        }

        private void AddEnPassantCaptures(List<Action> actions, Square? diagonalLeft, Square? diagonalRight, Action? previousAction)
        {
            // can en passant capture if the previous action was a PawnDoubleMove
            if (diagonalLeft != null &&
                ChessUtils.CanEnPassant(TeamColour, diagonalLeft, previousAction))
            {
                actions.Add(new Action(this, diagonalLeft, ActionType.PawnEnPassant));
            }

            if (diagonalRight != null &&
                ChessUtils.CanEnPassant(TeamColour, diagonalRight, previousAction))
            {
                actions.Add(new Action(this, diagonalRight, ActionType.PawnEnPassant));
            }
        }
    }
}
