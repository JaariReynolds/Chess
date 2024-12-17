using Chess.Classes;
using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace ChessLogic.Classes
{
    public static class ForsythEdwardsNotation
    {

        private static string validCharacters = "kKQqRrBbNnPp";
        private static string validCastles = "KkQq-";

        public static string GenerateFen(Gameboard gameboard)
        {
            throw new NotImplementedException();
        }

        public static Gameboard ParseFen(string fen)
        {
            var result = ValidateFen(fen);

            if (!result.IsValid)
                throw new ArgumentException(result.ErrorReason);

            var gameboard = new Gameboard();

            string[] parts = fen.Split(" ");

            // all 'parts' assumed to be non-empty, valid FEN strings (but not necessarily valid 
            gameboard.Board = ParseBoard(parts[0]);
            gameboard.CurrentTeamColour = ParseTeamColour(parts[1]);
            ParseCastlingAvailability(parts[2], gameboard.Board); // updating board by reference
            gameboard.LastPerformedAction = ParseEnpassantTarget(parts[3], gameboard.Board);
            gameboard.HalfMoveCounter = ParseHalfMoveCounter(parts[4]);
            gameboard.FullMoveCounter = ParseFullMoveCounter(parts[5]);

            return gameboard;
        }

        public static ValidationResult ValidateFen(string fen)
        {
            if (string.IsNullOrEmpty(fen))
                return ValidationResult.Fail("FEN string is null or empty.");


            string[] parts = fen.Split(" ");
            if (parts.Length != 6)
                return ValidationResult.Fail($"FEN string does not contain 6 sections. Sections provided: {parts.Length}.");

            ValidationResult result;

            result = ValidatePiecePlacement(parts[0]);
            if (!result.IsValid) return result;

            result = ValidateActiveColour(parts[1]);
            if (!result.IsValid) return result;

            result = ValidateCastlingAvailability(parts[2]);
            if (!result.IsValid) return result;

            result = ValidateEnpassantTarget(parts[3]);
            if (!result.IsValid) return result;

            result = ValidateHalfMoveCounter(parts[4]);
            if (!result.IsValid) return result;

            result = ValidateFullMoveCounter(parts[5]);
            if (!result.IsValid) return result;

            return ValidationResult.Success();
        }

        private static bool isFenValidForGameboard(Gameboard gameboard, string fen)
        {
            throw new NotImplementedException();
        }

        public static bool IsEqualFen(string fen1, string fen2)
        {
            throw new NotImplementedException();
        }

        private static ValidationResult ValidatePiecePlacement(string piecePlacement)
        {
            // ensure valid length
            string[] ranks = piecePlacement.Split("/");
            if (ranks.Length != 8)
                return ValidationResult.Fail($"Piece placement section does not contain 8 ranks. Ranks provided: {ranks.Length}. ");

            // ensure valid characters and square count in every rank/row
            foreach (var rank in ranks)
            {
                int squareCount = 0;
                foreach (var character in rank)
                {
                    if (char.IsDigit(character))
                    {
                        int gap = character - '0'; // char to int
                        if (gap < 1 || gap > 8)
                            return ValidationResult.Fail($"Invalid gap value '{gap}' in rank: '{rank}'.");
                        squareCount += gap;
                    }
                    else if (validCharacters.Contains(character))
                        squareCount++;
                    else
                        return ValidationResult.Fail($"Invalid character '{character}' in rank: '{rank}'.");

                    if (squareCount > 8)
                        return ValidationResult.Fail($"Rank exceeds 8 squares: '{rank}'.");
                }
                if (squareCount != 8)
                    return ValidationResult.Fail($"Rank does not sum to 8 squares: '{rank}'.");
            }

            return ValidationResult.Success();
        }

        private static ValidationResult ValidateActiveColour(string activeColour)
        {
            if (activeColour.ToLower() != "w" && activeColour.ToLower() != "b")
                return ValidationResult.Fail($"Active colour `{activeColour}' is not valid.");

            return ValidationResult.Success();
        }

        private static ValidationResult ValidateCastlingAvailability(string castlingAvailability)
        {
            // 1. ensure valid length
            if (string.IsNullOrEmpty(castlingAvailability) || castlingAvailability.Length > 4)
                return ValidationResult.Fail($"Castling availability styring must have 1 to 4 characters: '{castlingAvailability}'.");

            // 2. ensure valid characters
            if (!castlingAvailability.All(character => validCastles.Contains(character)))
                return ValidationResult.Fail($"Invalid character in castling availability string: '{castlingAvailability}'.");

            // 3. ensure that, if a dash is included, the rest of the characters are still valid
            if (castlingAvailability.Contains('-'))
            {
                if (castlingAvailability == "-")
                    return ValidationResult.Success();
                if (castlingAvailability.Any(c => c != '-' && !char.IsLetter(c)))
                    return ValidationResult.Fail("If '-' is included, it should only replace a missing castling right.");
            }

            return ValidationResult.Success();
        }

        private static ValidationResult ValidateEnpassantTarget(string enpassantTarget)
        {
            if (enpassantTarget == "-")
                return ValidationResult.Success();

            if (enpassantTarget.Length != 2)
                return ValidationResult.Fail($"Invalid en passant square: '{enpassantTarget}'.");

            if (enpassantTarget[1] != '3' && enpassantTarget[1] != '6')
                return ValidationResult.Fail($"En passant targets can only occur on ranks 3 or 6: '{enpassantTarget}'.");

            // ensure the target is a valid square
            return ValidateAlgebraicNotation(enpassantTarget);
        }

        private static ValidationResult ValidateHalfMoveCounter(string halfMoveCounter)
        {
            try
            {
                var number = int.Parse(halfMoveCounter);

                return number < 0 
                    ? ValidationResult.Fail($"The half-move counter needs to be a non-negative integer: '{number}'.") 
                    : ValidationResult.Success();
            }
            catch (Exception)
            {
                return ValidationResult.Fail($"The half-move counter is not a valid integer: '{halfMoveCounter}'.");
            }
        }

        private static ValidationResult ValidateFullMoveCounter(string fullMoveCounter)
        {
            try
            {
                int number = int.Parse(fullMoveCounter);

                if (number <= 0)
                    return ValidationResult.Fail($"The full move counter value needs to be greater than 0: '{number}'.");

                return ValidationResult.Success();
            }
            catch (Exception)
            {
                return ValidationResult.Fail($"The full move counter value is not a valid integer: '{fullMoveCounter}'.");
            }
        }

        private static ValidationResult ValidateAlgebraicNotation(string algebraicNotation)
        {
            if (algebraicNotation.Length != 2)
                return ValidationResult.Fail($"Square `{algebraicNotation}` is invalid: it must be exactly 2 characters.");

            char file = algebraicNotation[0];
            char rank = algebraicNotation[1];

            if (file < 'a' || file > 'h')
                return ValidationResult.Fail($"Square `{algebraicNotation}` is invalid: the file `{file}` must be a letter between 'a' and 'h'.");

            if (rank < '1' || rank > '8')
                return ValidationResult.Fail($"Square `{algebraicNotation}` is invalid: the rank `{rank}` must be a number between '1' and '8'.");

            return ValidationResult.Success();
        }

        private static Piece[][] ParseBoard(string piecePlacement)
        {
            var board = ChessUtils.InitialiseBoard();
            var ranks = piecePlacement.Split("/");

            // foreach rank/row
            for (int x = 0; x < ranks.Length; x++)
            {
                int column = 0;
                // foreach file/column
                for (int y = 0; y < ranks[x].Length; y++)
                {
                    // skip squares equal to the number in the FEN string
                    if (char.IsDigit(ranks[x][y]))
                    {
                        int skipCount = int.Parse(ranks[x][y].ToString());
                        column += skipCount;
                    }

                    // initialise board square with corresponding piece
                    else
                    {
                        var squareString = AlgebraicNotationUtils.ToAlgebraicNotation(new Square(x, column));

                        board[x][column] = ranks[x][y] switch
                        {
                            'P' => new Pawn(TeamColour.White, squareString),
                            'p' => new Pawn(TeamColour.Black, squareString),
                            'N' => new Knight(TeamColour.White, squareString),
                            'n' => new Knight(TeamColour.Black, squareString),
                            'B' => new Bishop(TeamColour.White, squareString),
                            'b' => new Bishop(TeamColour.Black, squareString),
                            'R' => new Rook(TeamColour.White, squareString),
                            'r' => new Rook(TeamColour.Black, squareString),
                            'Q' => new Queen(TeamColour.White, squareString),
                            'q' => new Queen(TeamColour.Black, squareString),
                            'K' => new King(TeamColour.White, squareString),
                            'k' => new King(TeamColour.Black, squareString),
                            _ => throw new ArgumentException($"Unable to parse piece type: '{ranks[x][y]}'.")
                        };
                        column++;
                    }
                }
            }

            return board;
        }

        private static TeamColour ParseTeamColour(string activeColour)
        {
            var colour = activeColour.ToLower();

            return colour switch
            {
                "w" => TeamColour.White,
                "b" => TeamColour.Black,
                _ => throw new ArgumentException($"Unable to parse active colour: '{colour}'.")
            };
        }

        private static void ParseCastlingAvailability(string castlingAvailability, Piece[][] board)
        {
            // disable certain castling availability by setting the corresponding Rook.Hasmoved to true

            if (!castlingAvailability.Contains('K'))
            {
                // check rook on white kingside
                var piece = board.GetPieceAt("h1");
                if (piece is { Name: PieceName.Rook })
                    piece.HasMoved = true;
            }
            if (!castlingAvailability.Contains('Q'))
            {
                // check rook on white queenside
                var piece = board.GetPieceAt("a1");
                if (piece is { Name: PieceName.Rook })
                    piece.HasMoved = true;
            }
            if (!castlingAvailability.Contains('k'))
            {
                // check rook on black queenside
                var piece = board.GetPieceAt("h8");
                if (piece is { Name: PieceName.Rook })
                    piece.HasMoved = true;
            }
            if (!castlingAvailability.Contains('q'))
            {
                // check rook on black queenside
                var piece = board.GetPieceAt("a8");
                if (piece is { Name: PieceName.Rook })
                    piece.HasMoved = true;
            }
        }

        private static Action? ParseEnpassantTarget(string enpassantTarget, Piece[][] board)
        {
            if (enpassantTarget == "-")
                return null;

            var file = enpassantTarget[0];
            var rank = enpassantTarget[1];

            // en passant target on rank 3, so pawn should be TeamColour.Black on rank 4 of same file
            if (rank == '3')
            {
                var squareNotation = $"{file}4";
                var piece = board.GetPieceAt(squareNotation);
                if (piece is not { Name: PieceName.Pawn, TeamColour: TeamColour.Black })
                    return null;
                
                var initialPiece = new Pawn(TeamColour.Black, $"{file}2");
                var enpassantAction = new Action(initialPiece, $"{file}4", ActionType.PawnDoubleMove);
                return enpassantAction;
            }
            // en passant target on rank 6, so pawn should be TeamColour.White on rank 5 of same file
            if (rank == 6)
            {
                var squareNotation = $"{enpassantTarget[0]}5";
                var piece = board.GetPieceAt(squareNotation);
                if (piece is not { Name: PieceName.Pawn, TeamColour: TeamColour.White }) 
                    return null;
                
                var initialPiece = new Pawn(TeamColour.White, $"{file}7");
                var enpassantAction = new Action(initialPiece, $"{file}5", ActionType.PawnDoubleMove);
                return enpassantAction;
            }

            return null;
        }

        private static int ParseHalfMoveCounter(string halfMoveCounter)
        {
            return int.Parse(halfMoveCounter);
        }

        private static int ParseFullMoveCounter(string fullMoveCounter)
        {
            return int.Parse(fullMoveCounter);
        }
    }
}
