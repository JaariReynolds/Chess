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
            bool deadNorth = false, deadSouth = false, deadEast = false, deadWest = false;

            for (int i = 1; i <= 7; i++)
            {
                if (!deadNorth)
                    deadNorth = ChessUtils.DeterminePieceAction(this, actions, PositionX - i, PositionY, boardState); 
                if (!deadSouth)
                    deadSouth = ChessUtils.DeterminePieceAction(this, actions, PositionX + i, PositionY, boardState); 
                if (!deadEast)
                    deadEast = ChessUtils.DeterminePieceAction(this, actions, PositionX, PositionY + i, boardState); 
                if (!deadWest)
                    deadWest = ChessUtils.DeterminePieceAction(this, actions, PositionX, PositionY - i, boardState); 
            }
         
            return actions;
        }
    }
}
