using Chess.Types;

namespace Chess.Classes
{
    public abstract class Piece
    {
        public Square Square { get; set; }
        public TeamColour TeamColour { get; set; }
        public int PieceValue { get; protected set; }

        public abstract void Draw();
        public abstract List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction);
        public abstract Piece Clone();

        public Piece(TeamColour teamColour, int x, int y)
        {
            TeamColour = teamColour;
            Square = new Square(x, y);

        }

        public void MovePiece(int x, int y)
        {
            Square.X = x;
            Square.Y = y;
        }

        public override string ToString()
        {
            return $"{TeamColour} {GetType().Name} at {Square}";
        }

        public override bool Equals(object? obj)
        {
            var piece = obj as Piece;
            if (piece == null) return false;

            return ToString().Equals(piece.ToString());
        }

        public override int GetHashCode()
        {
            unchecked // overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + TeamColour.GetHashCode();
                hash = hash * 23 + Square.X.GetHashCode();
                hash = hash * 23 + Square.Y.GetHashCode();
                hash = hash * 23 + GetType().Name.GetHashCode();
                return hash;
            }
        }
    }
}
