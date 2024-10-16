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

        private string rank;

        public King(TeamColour teamColour, string algebraicNotation, bool hasMoved) : base(teamColour, algebraicNotation, hasMoved, "King", -1)
        {
            rank = teamColour == TeamColour.White ? "1" : "8";
        }
        public King(TeamColour teamColour, string algebraicNotation) : this(teamColour, algebraicNotation, false) { }

        public override Piece Clone()
        {
            return new King(TeamColour, Square.ToString(), HasMoved);
        }

        public override void Draw()
        {
            Console.Write(TeamColour == TeamColour.White ? " K " : " k ");
        }

        public override List<Action> GetPotentialActions(Piece[][] boardState, Action? lastPerformedAction, bool includeCastles)
        {
            // 8 surrounding squares 
            var actions = new List<Action>();

            for (int i = 0; i < moves.GetLength(0); i++)
            {
                int newX = Square.X + moves[i, 0];
                int newY = Square.Y + moves[i, 1];

                if (!ChessUtils.IsWithinBounds(newX, newY))
                    continue;

                ChessUtils.DeterminePieceAction(this, actions, new Square(newX, newY), boardState);
            }

            // Castling requires
            // 1. Neither the king nor the rook has previously moved
            // 2. There are no pieces between the king and the rook
            // 3. The king is not currently in check
            // 4. The king does not pass through or finish on a square that is attacked by an enemy piece
            if (includeCastles)
            {
                if (CanKingsideCastle(boardState, lastPerformedAction))
                    actions.Add(new Action(this, $"g{rank}", ActionType.KingsideCastle));

                if (CanQueensideCastle(boardState, lastPerformedAction))
                    actions.Add(new Action(this, $"c{rank}", ActionType.QueensideCastle));
            }

            return actions;
        }

        public bool CanKingsideCastle(Piece[][] boardState, Action? lastPerformedAction)
        {
            // 1. King cannot have moved
            if (HasMoved || Square.ToString() != $"e{rank}") return false;

            // 1. Rook cannot have moved
            var whiteRook = boardState.GetPieceAt($"h{rank}");
            if (whiteRook == null || whiteRook!.HasMoved || whiteRook.Square.ToString() != $"h{rank}") return false;

            // 2. Empty squares between King and Rook (f1/8, g1/8)
            if (boardState.GetPieceAt($"f{rank}") != null ||
                boardState.GetPieceAt($"g{rank}") != null)
                return false;

            // 3/4. opposing team cannot be attacking squares e1/8 (king), f1/8 (empty), g1/8 (empty)
            var enemyActions = boardState.GetAllPossibleActions(TeamColour.GetOppositeTeam(), lastPerformedAction, false);
            string[] safeSquares = { $"e{rank}", $"f{rank}", $"g{rank}" };

            foreach (var action in enemyActions)
                if (safeSquares.Contains(action.Square.ToString()))
                    return false;

            // guards passed, can castle
            return true;
        }

        public bool CanQueensideCastle(Piece[][] boardState, Action? lastPerformedAction)
        {
            // 1. King cannot have moved
            if (HasMoved) return false;

            // 1. Rook cannot have moved
            var whiteRook = boardState.GetPieceAt($"a{rank}");
            if (whiteRook == null || whiteRook!.HasMoved) return false;

            // 2. Empty squares between King and Rook (b1/8, c1/8, d1/8)
            if (boardState.GetPieceAt($"b{rank}") != null ||
                boardState.GetPieceAt($"c{rank}") != null ||
                boardState.GetPieceAt($"d{rank}") != null)
                return false;

            // 3/4. opposing team cannot be attacking c1/8 (empty), d1/8 (empty), e1/8 (king)
            // opposing team CAN be attacking b1/8, as only the rook passes through this square. castling legality does not care about the rook being under threat
            var enemyActions = boardState.GetAllPossibleActions(TeamColour.GetOppositeTeam(), lastPerformedAction, false);
            string[] safeSquares = { $"c{rank}", $"d{rank}", $"e{rank}" };

            foreach (var action in enemyActions)
                if (safeSquares.Contains(action.Square.ToString()))
                    return false;

            // guards passed, can castle
            return true;
        }
    }
}
