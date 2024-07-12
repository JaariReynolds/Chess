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
    }
}
