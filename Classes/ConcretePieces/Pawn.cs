using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Pawn : Piece
    {
        private int startX;
        private int lastRow;
        private int actionAmount;

        public Pawn(TeamColour teamColour, string algebraicNotation) : base(teamColour, algebraicNotation)
        {
            PieceValue = 1;
            startX = teamColour == TeamColour.White ? 6 : 1;
            lastRow = teamColour == TeamColour.White ? 0 : 7;
            actionAmount = teamColour == TeamColour.White ? -1 : 1;
        }

        public Pawn(TeamColour teamColour, int x, int y) : this(teamColour, ChessUtils.ToAlgebraicNotation(x, y))
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

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            var actions = new List<Action>();

            // can move 2 squares on the first move, if no obstruction
            if (Square.X == startX
                && ChessUtils.IsEmptySquare(Square.X + actionAmount, Square.Y, boardState)
                && ChessUtils.IsEmptySquare(Square.X + (actionAmount * 2), Square.Y, boardState))
                actions.Add(new Action(this, Square.X + (actionAmount * 2), Square.Y, ActionType.PawnDoubleMove));

            // can move 1 at any point, but not capture
            if (ChessUtils.IsEmptySquare(Square.X + actionAmount, Square.Y, boardState))
                // promote if moving to the last row
                if (Square.X + actionAmount == lastRow)
                    AddPromoteActions(actions, Square.X + actionAmount, Square.Y);
                else
                    actions.Add(new Action(this, Square.X + actionAmount, Square.Y, ActionType.Move));

            // Since a Pawn can move/capture + promote on the same action, promote takes priority

            // can capture 1 diagonally
            if (ChessUtils.IsEnemy(TeamColour, Square.X + actionAmount, Square.Y - 1, boardState))
                // promote if capturing on the last row, otherwise capture
                if (Square.X + actionAmount == lastRow)
                    AddPromoteActions(actions, Square.X + actionAmount, Square.Y - 1);
                else
                    actions.Add(new Action(this, Square.X + actionAmount, Square.Y - 1, ActionType.Capture));

            if (ChessUtils.IsEnemy(TeamColour, Square.X + actionAmount, Square.Y + 1, boardState))
                if (Square.X + actionAmount == lastRow)
                    AddPromoteActions(actions, Square.X + actionAmount, Square.Y + 1);
                else
                    actions.Add(new Action(this, Square.X + actionAmount, Square.Y + 1, ActionType.Capture));

            // can enpassant capture if the previous action was a PawnDoubleMove
            if (ChessUtils.EnPassant(TeamColour, Square.X + actionAmount, Square.Y - 1, lastPerformedAction))
                actions.Add(new Action(this, Square.X + actionAmount, Square.Y - 1, ActionType.PawnEnPassant));

            if (ChessUtils.EnPassant(TeamColour, Square.X + actionAmount, Square.Y + 1, lastPerformedAction))
                actions.Add(new Action(this, Square.X + actionAmount, Square.Y + 1, ActionType.PawnEnPassant));
            return actions;
        }

        public void AddPromoteActions(List<Action> actions, int actionX, int actionY)
        {
            actions.Add(new Action(this, actionX, actionY, ActionType.PawnPromoteKnight));
            actions.Add(new Action(this, actionX, actionY, ActionType.PawnPromoteBishop));
            actions.Add(new Action(this, actionX, actionY, ActionType.PawnPromoteRook));
            actions.Add(new Action(this, actionX, actionY, ActionType.PawnPromoteQueen));
        }
    }
}
