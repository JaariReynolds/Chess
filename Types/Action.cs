using Chess.Classes;
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
    PawnPromoteQueen
}

public class Action
{
    public Piece Piece { get; private set; }
    public Square Square { get; private set; }
    public ActionType ActionType { get; private set; }

    public Action(Piece piece, int actionX, int actionY, ActionType actionType)
    {
        Piece = piece;
        Square = new Square(actionX, actionY);
        ActionType = actionType;
    }

    public Action(Piece piece, string algebraicNotation, ActionType actionType)
    {
        Piece = piece;
        var (x, y) = ChessUtils.CoordsFromAlgebraicNotation(algebraicNotation);
        Square = new Square(x, y);
        ActionType = actionType;
    }

    public Action(Action existingAction)
    {
        Piece = existingAction.Piece.Clone();
        Square = existingAction.Square;
        ActionType = existingAction.ActionType;
    }

    public override string ToString()
    {
        return $"{Piece} - {ActionType}: {Square}";
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
