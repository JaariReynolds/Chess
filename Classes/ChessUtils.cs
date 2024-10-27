using Chess.Types;

namespace Chess.Classes
{
    public static class ChessUtils
    {
        public static bool IsWithinBounds(Square square)
        {
            return square.X >= 0 && square.X < 8 && square.Y >= 0 && square.Y < 8;
        }

        public static bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < 8 && y >= 0 && y < 8;
        }

        public static bool IsNotNull(Square square, Piece[][] boardState)
        {
            return IsWithinBounds(square) && boardState[square.X][square.Y] != null;
        }

        public static bool IsEmptySquare(Square square, Piece[][] boardState)
        {
            return IsWithinBounds(square) && boardState[square.X][square.Y] == null;
        }

        public static bool IsEnemy(TeamColour friendlyTeamColour, Square square, Piece[][] boardState)
        {
            return IsNotNull(square, boardState) && boardState[square.X][square.Y].TeamColour != friendlyTeamColour;
        }

        public static bool IsPawnPromoteAction(Action action)
        {
            return
                action.ActionType == ActionType.PawnPromoteKnight ||
                action.ActionType == ActionType.PawnPromoteBishop ||
                action.ActionType == ActionType.PawnPromoteRook ||
                action.ActionType == ActionType.PawnPromoteQueen;
        }

        public static bool CanEnPassant(TeamColour currentTeamColour, Square square, Action? lastPerformedAction)
        {
            bool canEnPassant = false;

            // en passant captures only valid when the last performed move is a pawn double move
            if (lastPerformedAction == null || lastPerformedAction.ActionType != ActionType.PawnDoubleMove)
                return canEnPassant;

            switch (currentTeamColour)
            {
                // White en passant can only be performed on row index 2
                case TeamColour.White:
                    if (square.X == 2 && square.Y == lastPerformedAction.Square.Y)
                        canEnPassant = true;
                    break;

                // Black en passant can only be performed on row index 5
                case TeamColour.Black:
                    if (square.X == 5 && square.Y == lastPerformedAction.Square.Y)
                        canEnPassant = true;
                    break;
            }

            return canEnPassant;
        }

        public static bool DeterminePieceAction(Piece piece, List<Action> actions, Square square, Piece[][] boardState)
        {
            // deadEnd used for Rook, Bishop, Queen, where their directional moves stop after an obstruction

            bool deadEnd = true;
            if (IsEmptySquare(square, boardState))
            {
                actions.Add(new Action(piece, square, ActionType.Move));
                deadEnd = false;
            }
            else if (IsEnemy(piece.TeamColour, square, boardState))
            {
                actions.Add(new Action(piece, square, ActionType.Capture));
            }

            return deadEnd;
        }

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

        public static string ToAlgebraicNotation(Action action)
        {
            string pieceAbbreviation = action.Piece.PieceAbbreviation;

            if (action.Piece.Name == PieceName.Pawn) pieceAbbreviation = "";

            string moveNotation = action.ActionType switch
            {
                ActionType.Move or ActionType.PawnDoubleMove => $"{pieceAbbreviation}{action.Square}",
                ActionType.Capture or ActionType.PawnEnPassant =>
                    (action.Piece.Name == PieceName.Pawn)
                        ? $"{action.Piece.Square.ToString()[0]}x{action.Square}"
                        : $"{pieceAbbreviation}x{action.Square}",
                ActionType.PawnPromoteKnight =>
                    action.PromoteCapturePoints > 0
                        ? $"{action.Piece.Square.ToString()[0]}x{action.Square}={GetPieceAbbreviation(PieceName.Knight, action.Piece.TeamColour)}"
                        : $"{action.Square}={GetPieceAbbreviation(PieceName.Knight, action.Piece.TeamColour)}",
                ActionType.PawnPromoteBishop =>
                    action.PromoteCapturePoints > 0
                        ? $"{action.Piece.Square.ToString()[0]}x{action.Square}={GetPieceAbbreviation(PieceName.Bishop, action.Piece.TeamColour)}"
                        : $"{action.Square}={GetPieceAbbreviation(PieceName.Bishop, action.Piece.TeamColour)}",
                ActionType.PawnPromoteRook =>
                    action.PromoteCapturePoints > 0
                        ? $"{action.Piece.Square.ToString()[0]}x{action.Square}={GetPieceAbbreviation(PieceName.Rook, action.Piece.TeamColour)}"
                        : $"{action.Square}={GetPieceAbbreviation(PieceName.Rook, action.Piece.TeamColour)}",
                ActionType.PawnPromoteQueen =>
                    action.PromoteCapturePoints > 0
                        ? $"{action.Piece.Square.ToString()[0]}x{action.Square}={GetPieceAbbreviation(PieceName.Queen, action.Piece.TeamColour)}"
                        : $"{action.Square}={GetPieceAbbreviation(PieceName.Queen, action.Piece.TeamColour)}",
                ActionType.KingsideCastle => "O-O",
                ActionType.QueensideCastle => "O-O-O",
            };

            return moveNotation;
        }

        public static string ToAlgebraicNotation(Piece piece, Square targetSquare, ActionType actionType, int promoteCapturePoints)
        {
            string pieceAbbreviation = piece.PieceAbbreviation;

            if (piece.Name == PieceName.Pawn) pieceAbbreviation = "";

            if (actionType == ActionType.KingsideCastle)
            {

            }

            string moveNotation = actionType switch
            {
                ActionType.Move or ActionType.PawnDoubleMove => $"{pieceAbbreviation}{targetSquare}",
                ActionType.Capture or ActionType.PawnEnPassant =>
                    (piece.Name == PieceName.Pawn)
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}"
                        : $"{pieceAbbreviation}x{targetSquare}",
                ActionType.PawnPromoteKnight =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Knight, piece.TeamColour)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Knight, piece.TeamColour)}",
                ActionType.PawnPromoteBishop =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Bishop, piece.TeamColour)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Bishop, piece.TeamColour)}",
                ActionType.PawnPromoteRook =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Rook, piece.TeamColour)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Rook, piece.TeamColour)}",
                ActionType.PawnPromoteQueen =>
                    promoteCapturePoints > 0
                        ? $"{piece.Square.ToString()[0]}x{targetSquare}={GetPieceAbbreviation(PieceName.Queen, piece.TeamColour)}"
                        : $"{targetSquare}={GetPieceAbbreviation(PieceName.Queen, piece.TeamColour)}",
                ActionType.KingsideCastle => "O-O",
                ActionType.QueensideCastle => "O-O-O",
            };

            return moveNotation;
        }

        public static Piece[][] InitialiseBoard()
        {
            var board = new Piece[8][];

            for (int i = 0; i < 8; i++)
                board[i] = new Piece[8];

            return board;
        }

        public static string GetPieceAbbreviation(PieceName pieceName, TeamColour teamColour)
        {
            string piece = "";
            switch (pieceName)
            {
                case PieceName.Pawn:
                    piece = "P";
                    break;
                case PieceName.Knight:
                    piece = "N";
                    break;
                case PieceName.Rook:
                    piece = "R";
                    break;
                case PieceName.Bishop:
                    piece = "B";
                    break;
                case PieceName.King:
                    piece = "K";
                    break;
                case PieceName.Queen:
                    piece = "Q";
                    break;
            }

            return teamColour == TeamColour.White ? piece : piece.ToLower();
        }
    }
}
