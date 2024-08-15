using Chess.Types;

namespace Chess.Classes
{
    public abstract class Piece
    {
        public string Name { get; set; }
        public Square Square { get; set; }
        public TeamColour TeamColour { get; set; }
        public int PieceValue { get; set; }
        public bool HasMoved { get; set; }
        public abstract void Draw();

        /// <summary>
        /// Return a list of actions available for the provided piece.
        /// </summary>
        /// <param name="lastPerformedAction">Only relevant for Pawn en passant.</param>
        /// <param name="includeCastles">Only relevant to exclude castles when calling this method from a King's castle method.</param>
        public abstract List<Action> GetPotentialActions(Piece[][] boardState, Action? lastPerformedAction, bool includeCastles);

        public abstract Piece Clone();

        public Piece(TeamColour teamColour, string algebraicNotation, bool hasMoved, string name, int pieceValue)
        {
            Name = name;
            TeamColour = teamColour;
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            Square = new Square(x, y);
            HasMoved = hasMoved;
            PieceValue = pieceValue;
        }



        // Deserialization only, properties will be overwritten
        public Piece()
        {
            Name = "";
            TeamColour = TeamColour.White;
            Square = new Square(0, 0);
            HasMoved = false;
            PieceValue = -1;
        }



        public Piece(string name, Square square, TeamColour teamColour, int pieceValue, bool hasMoved)
        {
            Name = name;
            Square = square;
            TeamColour = teamColour;
            PieceValue = pieceValue;
            HasMoved = hasMoved;
        }

        public void MovePiece(Square square)
        {
            var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(square.ToString());
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
