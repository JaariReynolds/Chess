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
        public string ToAlgebraicNotation()
        {
            char file = (char)('a' + X);
            int rank = 8 - Y;
            return $"{file}{rank}";
        }

        public override string ToString()
        {
            return ToAlgebraicNotation();
        }

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
