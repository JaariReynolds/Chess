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
                PieceName.Queen => "Q"
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
    }
}
