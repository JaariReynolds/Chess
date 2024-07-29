using Chess.Types;

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

        public Knight(TeamColour teamColour, string algebraicNotation) : base(teamColour, algebraicNotation)
        {
            PieceValue = 3;
        }

        public Knight(TeamColour teamColour, int x, int y) : this(teamColour, ChessUtils.ToAlgebraicNotation(x, y))
        {
        }

        public override Piece Clone()
        {
            return new Knight(TeamColour, Square.ToString());
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " N " : " n ");
        }

        public override List<Action> GetPotentialActions(Piece[][] boardState, Action? lastPerformedAction)
        {
            // A knight can move 2 vertically and 1 horizontally, or 1 vertically and 2 horizontally
            var actions = new List<Action>();

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int newX = Square.X + moves[i, 0];
                int newY = Square.Y + moves[i, 1];

                if (ChessUtils.IsEmptySquare(newX, newY, boardState))
                    actions.Add(new Action(this, newX, newY, ActionType.Move));
                else if (ChessUtils.IsEnemy(TeamColour, newX, newY, boardState))
                    actions.Add(new Action(this, newX, newY, ActionType.Capture));
            }

            return actions;
        }
    }
}
