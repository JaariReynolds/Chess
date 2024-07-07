using Chess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Classes.ConcretePieces
{
    public class Rook : Piece
    {
        public Rook(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
            PieceValue = 5;
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " R " : " r ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            List<Action> actions = new List<Action>();

            // north squares
            for (int y = PositionY - 1; y >= 0; y--)
            {
                bool deadEnd = DetermineRookAction(actions, PositionX, y, boardState);
                if (deadEnd) break;
            }

            // south squares
            for (int y = PositionY + 1; y <= 7; y++)
            {
                bool deadEnd = DetermineRookAction(actions, PositionX, y, boardState);
                if (deadEnd) break;
            }

            // east squares
            for (int x = PositionX + 1; x <= 7; x++)
            {
                bool deadEnd = DetermineRookAction(actions, x, PositionY, boardState);
                if (deadEnd) break;
            }

            // west squares
            for (int x = PositionX - 1; x >= 0; x--)
            {
                bool deadEnd = DetermineRookAction(actions, x, PositionY, boardState);
                if (deadEnd) break;
            }

            return actions;
        }

        private bool DetermineRookAction(List<Action> actions, int x, int y, Piece[,] boardState)
        {
            // a rook can move horizontally or vertically until capture or until blocked by a friendly piece

            bool deadEnd = true;
            if (ChessUtils.IsEmptySquare(x, y, boardState))
            {
                actions.Add(new Action(this, x, y, ActionType.Move));
                return false;
            }
            else if (ChessUtils.IsEnemy(TeamColour, x, y, boardState))
            {
                actions.Add(new Action(this, x, y, ActionType.Capture));
            }

            return deadEnd;
        }
    }
}
