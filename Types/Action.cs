using Chess.Classes;
using Chess.Classes.ConcretePieces;
using Chess.Types;

public enum ActionType
{
    Move,
    Capture,
    PawnDoubleMove,
    PawnEnPassant,
    PawnPromoteKnight,
    PawnPromoteBishop,
    PawnPromoteRook,
    PawnPromoteQueen,
    KingsideCastle,
    QueensideCastle,
}

public class Action
{
    public Piece Piece { get; set; }
    public Square Square { get; set; }
    public ActionType ActionType { get; set; }
    public int? PromoteCapturePoints { get; set; } = 0; // to only be used on Pawn promote + capture actions
    public string AlgebraicNotation { get; set; }

    // Deserialization only, properties will be overwritten
    public Action()
    {
        Piece = new Pawn();
        Square = new Square();
        ActionType = ActionType.Move;
        PromoteCapturePoints = 0;
    }

    public Action(Piece piece, Square square, ActionType actionType, int? promoteCapturePoints)
    {
        Piece = piece;
        AlgebraicNotation = ChessUtils.ToAlgebraicNotation(piece, square, actionType, promoteCapturePoints ?? 0);
        Square = square;
        ActionType = actionType;
        PromoteCapturePoints = promoteCapturePoints ?? 0;
    }

    public Action(Piece piece, string algebraicNotation, ActionType actionType, int? promoteCapturePoints)
    {
        Piece = piece;
        var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
        AlgebraicNotation = ChessUtils.ToAlgebraicNotation(piece, new Square(x, y), actionType, promoteCapturePoints ?? 0);
        Square = new Square(x, y);
        ActionType = actionType;
        PromoteCapturePoints = promoteCapturePoints ?? 0;
    }

    public Action(Piece piece, Square square, ActionType actionType) : this(piece, square, actionType, null) { }

    public Action(Piece piece, string algebraicNotation, ActionType actionType) : this(piece, algebraicNotation, actionType, null) { }

    public Action(Action existingAction)
    {
        Piece = existingAction.Piece.Clone();
        AlgebraicNotation = existingAction.AlgebraicNotation;
        Square = existingAction.Square;
        ActionType = existingAction.ActionType;
        PromoteCapturePoints = existingAction.PromoteCapturePoints ?? 0;
    }

    public override string ToString()
    {
        return AlgebraicNotation;
    }

    public override bool Equals(object? obj)
    {
        var action = obj as Action;
        if (action == null) return false;

        return ToString().Equals(action.ToString());
    }
    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 19;
            hash = hash * 25 + Square.X.GetHashCode();
            hash = hash * 25 + Square.Y.GetHashCode();
            hash = hash * 25 + ActionType.GetHashCode();
            return hash;
        }
    }
}
