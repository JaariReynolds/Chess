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
        private readonly int[,] directions = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1}, // N, S, E, W
        };

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
            // Cardinal direction movements only (i.e. N,S,E,W only)
            var actions = new List<Action>();

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                for (int distance = 1; distance <= 7; distance++)
                {
                    int newX = PositionX + directions[i, 0] * distance;
                    int newY = PositionY + directions[i, 1] * distance;
                    var deadEnd = ChessUtils.DeterminePieceAction(this, actions, newX, newY, boardState);
                    if (deadEnd) break;
                }
            }

            return actions;
        }
    }
}
