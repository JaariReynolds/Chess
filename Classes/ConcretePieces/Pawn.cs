using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Pawn : Piece
    {
        private int startX;
        private int lastRow;
        private int actionAmount;

        internal Pawn() : base() { }

        public Pawn(TeamColour teamColour, string algebraicNotation) : base(teamColour, algebraicNotation, "Pawn", 1)
        {
            startX = teamColour == TeamColour.White ? 6 : 1;
            lastRow = teamColour == TeamColour.White ? 0 : 7;
            actionAmount = teamColour == TeamColour.White ? -1 : 1;
        }

        public Pawn(TeamColour teamColour, int x, int y) : this(teamColour, ChessUtils.ToAlgebraicNotation(new Square(x, y)))
        {
        }

        public override Piece Clone()
        {
            return new Pawn(TeamColour, Square.ToString());
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " P " : " p ");
        }

        public override List<Action> GetPotentialActions(Piece[][] boardState, Action? lastPerformedAction)
        {
            var actions = new List<Action>();

            var oneSquareForward = new Square(Square.X + actionAmount, Square.Y);
            var twoSquaresForward = new Square(Square.X + (actionAmount * 2), Square.Y);
            var diagonalLeft = new Square(Square.X + actionAmount, Square.Y - 1);
            var diagonalRight = new Square(Square.X + actionAmount, Square.Y + 1);

            // can move 1 at any point, but not capture
            if (ChessUtils.IsEmptySquare(oneSquareForward, boardState))
                if (Square.X + actionAmount == lastRow) // promote if moving to the last row
                    AddPromoteActions(actions, oneSquareForward);
                else
                    actions.Add(new Action(this, oneSquareForward, ActionType.Move));

            // can move 2 squares on the first move, if no obstruction
            if (Square.X == startX && ChessUtils.IsEmptySquare(oneSquareForward, boardState) && ChessUtils.IsEmptySquare(twoSquaresForward, boardState))
                actions.Add(new Action(this, twoSquaresForward, ActionType.PawnDoubleMove));

            // Since a Pawn can move/capture + promote on the same action, promote takes priority

            // can capture 1 diagonally left
            if (ChessUtils.IsEnemy(TeamColour, diagonalLeft, boardState))
                if (Square.X + actionAmount == lastRow)
                    AddPromoteActions(actions, diagonalLeft);
                else
                    actions.Add(new Action(this, diagonalLeft, ActionType.Capture));

            // can capture 1 diagonally right
            if (ChessUtils.IsEnemy(TeamColour, diagonalRight, boardState))
                if (Square.X + actionAmount == lastRow) // promote if capturing on the last row, otherwise capture
                    AddPromoteActions(actions, diagonalRight);
                else
                    actions.Add(new Action(this, diagonalRight, ActionType.Capture));

            // can enpassant capture if the previous action was a PawnDoubleMove
            if (ChessUtils.EnPassant(TeamColour, diagonalLeft, lastPerformedAction))
                actions.Add(new Action(this, diagonalLeft, ActionType.PawnEnPassant));

            if (ChessUtils.EnPassant(TeamColour, diagonalRight, lastPerformedAction))
                actions.Add(new Action(this, diagonalRight, ActionType.PawnEnPassant));
            return actions;
        }

        public void AddPromoteActions(List<Action> actions, Square square)
        {
            actions.Add(new Action(this, square, ActionType.PawnPromoteKnight));
            actions.Add(new Action(this, square, ActionType.PawnPromoteBishop));
            actions.Add(new Action(this, square, ActionType.PawnPromoteRook));
            actions.Add(new Action(this, square, ActionType.PawnPromoteQueen));
        }
    }
}
