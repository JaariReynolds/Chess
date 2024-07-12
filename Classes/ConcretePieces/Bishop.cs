using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Bishop : Piece
    {
        private readonly int[,] directions = new int[,]
        {
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1} // NW, NE, SW, SE
        };

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
            // Intercardinal directions only (i.e. diagonals only)
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
