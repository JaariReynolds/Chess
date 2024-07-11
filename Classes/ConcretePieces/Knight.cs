using Chess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Classes.ConcretePieces
{
    public class Knight : Piece
    {
        private readonly int[,] moves = new int[,]
        {
            { 2, 1 }, { 2, -1 },
            { -2, 1 }, { -2, -1 },
            { 1, 2 }, { 1, -2 },
            { -1, 2 },{ -1, -2 }
        };
        
        public Knight(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
            PieceValue = 3;
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " K " : " k ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            // A knight can move 2 vertically and 1 horizontally, or 1 vertically and 2 horizontally
            var actions = new List<Action>();    
            
            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int newX = PositionX + moves[i, 0];
                int newY = PositionY + moves[i, 1];

                if (ChessUtils.IsEmptySquare(newX, newY, boardState))
                    actions.Add(new Action(this, newX, newY, ActionType.Move));
                else if (ChessUtils.IsEnemy(TeamColour, newX, newY, boardState))
                    actions.Add(new Action(this, newX, newY, ActionType.Capture));
            }

            return actions;
        }
    }
}
