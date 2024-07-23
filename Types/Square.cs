using Chess.Classes;

namespace Chess.Types
{
    public class Square
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Square(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Square(Square existingSquare)
        {
            X = existingSquare.X;
            Y = existingSquare.Y;
        }

        public override string ToString()
        {
            return ChessUtils.ToAlgebraicNotation(X, Y);
        }

        // Squares are considered equal if their X and Y coordinates are the same.
        // Allows for object equality checks instead of having to do
        // (action.Square.X == piece.Square.X && action.Square.Y == piece.Square.Y)
        public override bool Equals(object? obj)
        {
            var otherSquare = obj as Square;
            if (otherSquare == null) return false;
            return X == otherSquare.X && Y == otherSquare.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Square left, Square right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Square left, Square right)
        {
            return !(left == right);
        }
    }
}
