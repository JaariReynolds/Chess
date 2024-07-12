﻿using Chess.Classes;
using Chess.Types;

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
    public Square Square { get; private set; }
    public ActionType ActionType { get; private set; }

    public Action(Piece piece, int actionX, int actionY, ActionType actionType)
    {
        Piece = piece;
        Square = new Square(actionX, actionY);
        ActionType = actionType;
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
