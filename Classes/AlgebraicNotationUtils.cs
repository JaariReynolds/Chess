using Chess.Classes;
using Chess.Types;

namespace ChessLogic.Classes
{
    public static class AlgebraicNotationUtils
    {
        public static (int, int) CoordsFromAlgebraicNotation(string algebraicNotation)
        {
            if (algebraicNotation.Length != 2)
                throw new ArgumentException("Algebraic notation must be exactly 2 characters.");

            char fileChar = algebraicNotation[0];
            char rankChar = algebraicNotation[1];

            if (fileChar < 'a' || fileChar > 'h' || rankChar < '1' || rankChar > '8')
                throw new ArgumentException($"{algebraicNotation} is not a valid square");

            int column = fileChar - 'a';
            int row = '8' - rankChar;
            return (row, column);
        }

        // Converts array indexes to standard algebraic notation used in Chess (a8 top left, h1 bottom right)
        public static string ToAlgebraicNotation(Square square)
        {
            char file = (char)('a' + square.Y);
            int rank = 8 - square.X;
            return $"{file}{rank}";
        }

        public static string ToAlgebraicNotation(Piece piece, Square targetSquare, ActionType actionType, int promoteCapturePoints)
        {
            string pieceAbbreviation = piece.PieceAbbreviation;

            if (piece.Name == PieceName.Pawn) pieceAbbreviation = "";

            string moveNotation = actionType switch
            {
                ActionType.Move or ActionType.PawnDoubleMove => $"{pieceAbbreviation}{targetSquare}",
                ActionType.Capture or ActionType.PawnEnPassant =>
                    (piece.Name == PieceName.Pawn)
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}"
                        : $"{pieceAbbreviation}x{targetSquare}",
                ActionType.PawnPromoteKnight =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Knight)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Knight)}",
                ActionType.PawnPromoteBishop =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Bishop)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Bishop)}",
                ActionType.PawnPromoteRook =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Rook)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Rook)}",
                ActionType.PawnPromoteQueen =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Queen)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Queen)}",
                ActionType.KingsideCastle => "O-O",
                ActionType.QueensideCastle => "O-O-O",
                _ => throw new NotImplementedException($"{actionType} not yet implemented.")
            };

            return moveNotation;
        }

        public static string GetPieceAbbreviation(PieceName pieceName)
        {
            return pieceName switch
            {
                PieceName.Pawn => "P",
                PieceName.Knight => "N",
                PieceName.Rook => "R",
                PieceName.Bishop => "B",
                PieceName.King => "K",
                PieceName.Queen => "Q",
                _ => throw new NotImplementedException()
            };
        }

        public static string AddAlgebraicNotationSuffix(string algebraicNotation, TeamColour? checkedTeamColour, TeamColour? checkmateTeamColour)
        {
            if (checkmateTeamColour != null && checkedTeamColour != null && checkmateTeamColour == checkedTeamColour)
                return algebraicNotation + "#";

            else if (checkedTeamColour != null)
                return algebraicNotation + "+";

            else return algebraicNotation;
        }

        public static void AlgebraicNotationAmbiguityResolution(List<Action> actions)
        {
            // dictionary to sort actions with same algebraic notation
            var actionDictionary = new Dictionary<string, List<Action>>();
            foreach (var action in actions)
            {
                if (!actionDictionary.ContainsKey(action.AlgebraicNotation))
                    actionDictionary[action.AlgebraicNotation] = new List<Action>();

                actionDictionary[action.AlgebraicNotation].Add(action);
            }

            foreach (var kvp in actionDictionary)
            {
                if (kvp.Value.Count > 1)
                    AmbiguityResolution(kvp.Value);
            }
        }

        // Currently only considers if there are only 2 ambiguous actions, rare cases of 3 or more are unhandled with this implementation
        private static void AmbiguityResolution(List<Action> ambiguousActions)
        {
            var initialFile = ambiguousActions[0].Piece.ToString()[0];
            var initialRank = ambiguousActions[0].Piece.ToString()[1];

            // 3 or more ambiguousActions will not resolve these lines accurately
            bool sameFile = ambiguousActions.All(action => action.Piece.ToString()[0] == initialFile);
            bool sameRank = ambiguousActions.All(action => action.Piece.ToString()[1] == initialRank);

            if (!sameFile && !sameRank) // no ambiguity
                return;
            else if (!sameFile) // only files are different, can use to disambiguate
                FileResolution(ambiguousActions);
            else if (!sameRank) // only ranks are different, can use to disambiguate
                RankResolution(ambiguousActions);
            else // files AND ranks are the same (individually), so use both to disambiguate
                FileRankResolution(ambiguousActions);
        }

        private static void FileResolution(List<Action> ambiguousActions)
        {
            //Square[0] = file;
            foreach (var ambiguousAction in ambiguousActions)
                ambiguousAction.AlgebraicNotation = ambiguousAction.AlgebraicNotation.Insert(1, ambiguousAction.Piece.Square.ToString()[0].ToString());
        }

        private static void RankResolution(List<Action> ambiguousActions)
        {
            //Square[1] = rank;
            foreach (var ambiguousAction in ambiguousActions)
                ambiguousAction.AlgebraicNotation = ambiguousAction.AlgebraicNotation.Insert(1, ambiguousAction.Piece.Square.ToString()[1].ToString());
        }

        private static void FileRankResolution(List<Action> ambiguousActions)
        {
            foreach (var ambiguousAction in ambiguousActions)
                ambiguousAction.AlgebraicNotation = ambiguousAction.AlgebraicNotation.Insert(1, ambiguousAction.Piece.Square.ToString().ToString());
        }
    }
}
