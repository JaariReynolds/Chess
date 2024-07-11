using Chess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Classes.ConcretePieces
{
    public class King : Piece
    {
        private readonly int[,] moves = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1}, // N, S, E, W
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1} // NW, NE, SW, SE
        };

        public King(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
            PieceValue = 0; // King piece is invaluable and also does not matter 
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " K " : " k ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            // 8 surrounding squares 
            var actions = new List<Action>();

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int newX = PositionX + moves[i, 0];
                int newY = PositionY + moves[i, 1];
                ChessUtils.DeterminePieceAction(this, actions, newX, newY, boardState); 
            }

            return actions;
        }
    }
}
