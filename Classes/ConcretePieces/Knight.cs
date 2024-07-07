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
        // A knight can move 2 vertically and 1 horizontally, or 1 vertically and 2 horizontally
        List<(int, int)> moveOffsets = new List<(int, int)>
        {
            (2, 1), (2, -1),
            (-2, 1), (-2, -1),
            (1, 2), (1, -2),
            (-1, 2), (-1, -2)
        };
        
        public Knight(TeamColour teamColour, int x, int y) : base(teamColour, x, y)
        {
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " K " : " k ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            List<Action> actions = new List<Action>();    
            
            foreach (var move in moveOffsets)
            {
                if (ChessUtils.IsEmptySquare(PositionX + move.Item1, PositionY + move.Item2, boardState))
                    actions.Add(new Action(this, PositionX + move.Item1, PositionY + move.Item2, ActionType.Move));
                else if (ChessUtils.IsEnemy(TeamColour, PositionX + move.Item1, PositionY + move.Item2, boardState))
                    actions.Add(new Action(this, PositionX + move.Item1, PositionY + move.Item2, ActionType.Capture));
            }

            return actions;
        }
    }
}
