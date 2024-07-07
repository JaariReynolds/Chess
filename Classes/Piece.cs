using Chess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Classes
{
    public abstract class Piece
    {
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public TeamColour TeamColour { get; set; }
        public int PieceValue { get; protected set; }

        public abstract void Draw();
        public abstract List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction);

        public Piece(TeamColour teamColour, int x, int y)
        {
            TeamColour = teamColour;
            PositionX = x;
            PositionY = y;
        }

        public void MovePiece(int x, int y)
        {
            PositionX = x;
            PositionY = y;
        }

        public override string ToString()
        {
            return $"{TeamColour} {GetType().Name} at ({PositionX},{PositionY})";
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
                hash = hash * 23 + PositionX.GetHashCode();
                hash = hash * 23 + PositionY.GetHashCode();
                hash = hash * 23 + GetType().Name.GetHashCode();
                return hash;
            }
        }
    }
}
