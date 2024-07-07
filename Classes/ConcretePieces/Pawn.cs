using Chess.Classes;
using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Pawn : Piece
    {
        private int startX;
        private int lastRow;
        private int actionAmount;

        public Pawn(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
            startX = teamColour == TeamColour.White ? 6 : 1;
            lastRow = teamColour == TeamColour.White ? 0 : 7;
            actionAmount = teamColour == TeamColour.White ? -1 : 1;
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            // Since a Pawn can move/capture + promote on the same action, promote takes priority
            List<Action> actions = new List<Action>();

            // can move 2 squares on the first move, if no obstruction
            if (PositionX == startX
                && ChessUtils.IsEmptySquare(PositionX + actionAmount, PositionY, boardState)
                && ChessUtils.IsEmptySquare(PositionX + (actionAmount * 2), PositionY, boardState))
                actions.Add(new Action(this, PositionX + (actionAmount * 2), PositionY, ActionType.PawnDoubleMove));

            // can move 1 at any point, but not capture
            if (ChessUtils.IsEmptySquare(PositionX + actionAmount, PositionY, boardState))
                // promote if moving to the last row
                actions.Add(new Action(this, PositionX + actionAmount, PositionY, (PositionX + actionAmount == lastRow) ? ActionType.PawnPromote : ActionType.Move));

            // can capture 1 diagonally
            if (ChessUtils.IsEnemy(TeamColour, PositionX + actionAmount, PositionY - 1, boardState))
                // promote if capturing on the last row, otherwise capture
                actions.Add(new Action(this, PositionX + actionAmount, PositionY - 1, (PositionX + actionAmount == lastRow) ? ActionType.PawnPromote : ActionType.Capture));

            if (ChessUtils.IsEnemy(TeamColour, PositionX + actionAmount, PositionY + 1, boardState))
                actions.Add(new Action(this, PositionX + actionAmount, PositionY + 1, (PositionX + actionAmount == lastRow) ? ActionType.PawnPromote : ActionType.Capture));

            // can enpassant capture if the previous action was a PawnDoubleMove
            if (ChessUtils.EnPassant(TeamColour, PositionX + actionAmount, PositionY - 1, lastPerformedAction))
                actions.Add(new Action(this, PositionX + actionAmount, PositionY - 1, ActionType.PawnEnPassant));

            if (ChessUtils.EnPassant(TeamColour, PositionX + actionAmount, PositionY + 1, lastPerformedAction))
                actions.Add(new Action(this, PositionX + actionAmount, PositionY + 1, ActionType.PawnEnPassant));
            return actions;
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " P " : " p ");
        }
    }
}
