using Chess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Classes.ConcretePieces
{
    public class Bishop : Piece
    {
        public Bishop(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
            PieceValue = 3;
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " B " : " b ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            // Rook movement logic, except its diagonal 
            List<Action> actions = new List<Action>();
            bool deadNorthEast = false, deadNorthWest = false, deadSouthEast = false, deadSouthWest = false;

            for (int i = 1; i <= 7; i++)
            {
                if (!deadNorthWest)
                    deadNorthWest = ChessUtils.DeterminePieceAction(this, actions, PositionX - i, PositionY - i, boardState);
                if (!deadNorthEast)
                    deadNorthEast = ChessUtils.DeterminePieceAction(this, actions, PositionX - i, PositionY + i, boardState);
                if (!deadSouthWest)
                    deadSouthWest = ChessUtils.DeterminePieceAction(this, actions, PositionX + i, PositionY - i, boardState);
                if (!deadSouthEast)
                    deadSouthEast = ChessUtils.DeterminePieceAction(this, actions, PositionX + i, PositionY + i, boardState);
            }

            return actions;
        }
    }
}
