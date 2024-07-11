﻿using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Queen : Piece
    {
        private readonly int[,] directions = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1}, // N, S, E, W
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1} // NW, NE, SW, SE
        };

        public Queen(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
            PieceValue = 9;
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " Q " : " q ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            // Cardinal and Intercardinal (Rook and Bishop combined)
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