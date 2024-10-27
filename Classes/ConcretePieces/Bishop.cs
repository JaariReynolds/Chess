using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class Bishop : Piece
    {
        private readonly int[,] directions = new int[,]
        {
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1} // NW, NE, SW, SE
        };

        public Bishop(TeamColour teamColour, string algebraicNotation, bool hasMoved) : base(teamColour, algebraicNotation, hasMoved, PieceName.Bishop, 3) { }
        public Bishop(TeamColour teamColour, string algebraicNotation) : this(teamColour, algebraicNotation, false) { }

        public override Piece Clone()
        {
            return new Bishop(TeamColour, Square.ToString(), HasMoved);
        }

        public override List<Action> GetPotentialActions(Piece[][] boardState, Action? lastPerformedAction, bool includeCastles)
        {
            // Intercardinal directions only (i.e. diagonals only)
            var actions = new List<Action>();

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                for (int distance = 1; distance <= 7; distance++)
                {
                    int newX = Square.X + directions[i, 0] * distance;
                    int newY = Square.Y + directions[i, 1] * distance;

                    if (!ChessUtils.IsWithinBounds(newX, newY))
                        continue;

                    var deadEnd = ChessUtils.DeterminePieceAction(this, actions, new Square(newX, newY), boardState);
                    if (deadEnd) break;
                }
            }

            return actions;
        }
    }
}
