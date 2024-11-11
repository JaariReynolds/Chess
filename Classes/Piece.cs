using Chess.Types;
using ChessLogic.Classes;

namespace Chess.Classes
{
    public abstract class Piece
    {
        public PieceName Name { get; set; }
        public string PieceAbbreviation { get; set; }
        public Square Square { get; set; }
        public TeamColour TeamColour { get; set; }
        public int PieceValue { get; set; }
        public bool HasMoved { get; set; }
        public void Draw()
        {
            Console.Write(" " + (TeamColour == TeamColour.Black ? PieceAbbreviation.ToLower() : PieceAbbreviation) + " ");
        }

        /// <summary>
        /// Return a list of actions available for the provided piece.
        /// </summary>
        /// <param name="lastPerformedAction">Only relevant for Pawn en passant.</param>
        /// <param name="includeCastles">Only relevant to exclude castles when calling this method from a King's castle method.</param>
        public abstract List<Action> GetPotentialActions(Piece[][] boardState, bool includeCastles);

        public abstract Piece Clone();

        public Piece(TeamColour teamColour, string algebraicNotation, bool hasMoved, PieceName name, int pieceValue)
        {
            Name = name;
            PieceAbbreviation = AlgebraicNotationUtils.GetPieceAbbreviation(name);
            TeamColour = teamColour;
            var (x, y) = AlgebraicNotationUtils.CoordsFromAlgebraicNotation(algebraicNotation);
            Square = new Square(x, y);
            HasMoved = hasMoved;
            PieceValue = pieceValue;
        }

        // Deserialization only, properties will be overwritten
        public Piece()
        {
            Name = PieceName.Pawn;
            TeamColour = TeamColour.White;
            Square = new Square(0, 0);
            HasMoved = false;
            PieceValue = -1;
        }

        public Piece(PieceName name, Square square, TeamColour teamColour, int pieceValue, bool hasMoved)
        {
            Name = name;
            PieceAbbreviation = AlgebraicNotationUtils.GetPieceAbbreviation(name);
            Square = square;
            TeamColour = teamColour;
            PieceValue = pieceValue;
            HasMoved = hasMoved;
        }

        public void MovePiece(Square square)
        {
            var (x, y) = AlgebraicNotationUtils.CoordsFromAlgebraicNotation(square.ToString());
            Square.X = x;
            Square.Y = y;
            HasMoved = true;
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
