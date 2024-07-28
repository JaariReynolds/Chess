using Chess.Types;

namespace Chess.Classes.ConcretePieces
{
    public class King : Piece
    {
        private readonly int[,] moves = new int[,]
        {
            {-1, 0}, {1, 0}, {0, -1}, {0, 1}, // N, S, E, W
            {-1, -1}, {-1, 1}, {1, -1}, {1, 1} // NW, NE, SW, SE
        };

        public King(TeamColour teamColour, string algebraicNotation) : base(teamColour, algebraicNotation)
        {
            PieceValue = 0; // King piece is invaluable and also does not matter 
        }

        public King(TeamColour teamColour, int x, int y) : this(teamColour, ChessUtils.ToAlgebraicNotation(x, y))
        {
        }

        public override Piece Clone()
        {
            return new King(TeamColour, Square.ToString());
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " K " : " k ");
        }

        public override List<Action> GetPotentialActions(Piece[,] boardState, Action? lastPerformedAction)
        {
            // 8 surrounding squares 
            var actions = new List<Action>();

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int newX = Square.X + moves[i, 0];
                int newY = Square.Y + moves[i, 1];
                ChessUtils.DeterminePieceAction(this, actions, newX, newY, boardState);
            }

            // Castling requires
            // 1. Neither the king nor the rook has previously moved
            // 2. There are no pieces between the king and the rook
            // 3. The king is not currently in check
            // 4. The king does not pass through or finish on a square that is attacked by an enemy piece
            DetermineKingsideCastle(actions, boardState, lastPerformedAction);
            DetermineQueensideCastle(actions, boardState, lastPerformedAction);

            return actions;
        }

        public void DetermineKingsideCastle(List<Action> actions, Piece[,] boardState, Action? lastPerformedAction)
        {
            // 1. King cannot have moved
            if (HasMoved) return;

            var rank = TeamColour == TeamColour.White ? "1" : "8";

            // 1. Rook cannot have moved
            var whiteRook = boardState.GetPieceAt($"h{rank}");
            if (whiteRook == null || whiteRook!.HasMoved) return;

            // 2. Empty squares between King and Rook (f1/8, g1/8)
            if (boardState.GetPieceAt($"f{rank}") != null ||
                boardState.GetPieceAt($"g{rank}") != null)
                return;

            // 3/4. opposing team cannot be attacking squares e1/8 (king), f1/8 (empty), g1/8 (empty)
            var enemyActions = boardState.GetAllPossibleActions(TeamColour.GetOppositeTeam(), lastPerformedAction);
            string[] safeSquares = { $"e{rank}", $"f{rank}", $"g{rank}" };

            foreach (var action in enemyActions)
                if (safeSquares.Contains(action.Square.ToString()))
                    return;

            // guards passed, can castle
            actions.Add(new Action(this, $"g{rank}", ActionType.KingsideCastle));
        }

        public void DetermineQueensideCastle(List<Action> actions, Piece[,] boardState, Action? lastPerformedAction)
        {
            // 1. King cannot have moved
            if (HasMoved) return;

            var rank = TeamColour == TeamColour.White ? "1" : "8";

            // 1. Rook cannot have moved
            var whiteRook = boardState.GetPieceAt($"a{rank}");
            if (whiteRook == null || whiteRook!.HasMoved) return;

            // 2. Empty squares between King and Rook (b1/8, c1/8, d1/8)
            if (boardState.GetPieceAt($"b{rank}") != null ||
                boardState.GetPieceAt($"c{rank}") != null ||
                boardState.GetPieceAt($"d{rank}") != null)
                return;

            // 3/4. opposing team cannot be attacking squares b1/8 (empty), c1/8 (empty), d1/8 (empty), e1/8 (king)
            var enemyActions = boardState.GetAllPossibleActions(TeamColour.GetOppositeTeam(), lastPerformedAction);
            string[] safeSquares = { $"b{rank}", $"c{rank}", $"d{rank}", $"e{rank}" };

            foreach (var action in enemyActions)
                if (safeSquares.Contains(action.Square.ToString()))
                    return;

            // guards passed, can castle
            actions.Add(new Action(this, $"c{rank}", ActionType.QueensideCastle));
        }
    }
}
