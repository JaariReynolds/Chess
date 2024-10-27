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

        public Knight(TeamColour teamColour, string algebraicNotation) : this(teamColour, algebraicNotation, false) { }

        public Knight(TeamColour teamColour, string algebraicNotation, bool hasMoved) : base(teamColour, algebraicNotation, hasMoved, PieceName.Knight, 3)
        {
        }

        public override Piece Clone()
        {
            return new Knight(TeamColour, Square.ToString(), HasMoved);
        }

        public override List<Action> GetPotentialActions(Piece[][] boardState, Action? lastPerformedAction, bool includeCastles)
        {
            // A knight can move 2 vertically and 1 horizontally, or 1 vertically and 2 horizontally
            var actions = new List<Action>();

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int newX = Square.X + moves[i, 0];
                int newY = Square.Y + moves[i, 1];

                if (!ChessUtils.IsWithinBounds(newX, newY))
                    continue;

                Square square = new Square(newX, newY);

                if (ChessUtils.IsEmptySquare(square, boardState))
                    actions.Add(new Action(this, square, ActionType.Move));
                else if (ChessUtils.IsEnemy(TeamColour, square, boardState))
                    actions.Add(new Action(this, square, ActionType.Capture));
            }

            return actions;
        }
    }
}
