using Chess;

public enum ActionType
{
    Move,
    Capture,
    PawnPromote,
    PawnDoubleMove,
    PawnEnPassant,
}

public class Action
{
    public Piece Piece { get; private set; }
    public int ActionX { get; private set; }
    public int ActionY { get; private set; }
    public ActionType ActionType { get; private set; }

    public Action(Piece piece, int actionX, int actionY, ActionType actionType)
    {
        Piece = piece;
        ActionX = actionX;
        ActionY = actionY;
        ActionType = actionType;
    }

    public override string ToString()
    {
        return $"{Piece} - {ActionType}: ({ActionX}, {ActionY})";
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
            hash = hash * 25 + ActionX.GetHashCode();
            hash = hash * 25 + ActionY.GetHashCode();
            hash = hash * 25 + ActionType.GetHashCode();
            return hash;
        }
    }
}
