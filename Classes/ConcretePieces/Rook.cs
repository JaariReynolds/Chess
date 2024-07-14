using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Rook : Piece
    {
        private readonly int[,] directions = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1}, // N, S, E, W
        };


        public Rook(TeamColour teamColour, string algebraicNotation) : base(teamColour, algebraicNotation)
        {
            PieceValue = 5;
        }

        public Rook(TeamColour teamColour, int x, int y) : this(teamColour, ChessUtils.ToAlgebraicNotation(x, y))
        {
        }

        public override Piece Clone()
        {
            return new Rook(TeamColour, Square.ToString());
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
                    int newX = Square.X + directions[i, 0] * distance;
                    int newY = Square.Y + directions[i, 1] * distance;
                    var deadEnd = ChessUtils.DeterminePieceAction(this, actions, newX, newY, boardState);
                    if (deadEnd) break;
                }
            }

            return actions;
        }
    }
}
