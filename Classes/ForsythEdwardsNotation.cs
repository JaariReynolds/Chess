using Chess.Classes;

namespace ChessLogic.Classes
{
    public static class ForsythEdwardsNotation
    {

        private static string validCharacters = "kKQqRrBbNnPp";
        private static string validCastles = "KkQq-";

        public static string GenerateFEN(Gameboard gameboard)
        {
            throw new NotImplementedException();
        }

        public static Gameboard ParseFEN(string fen)
        {
            throw new NotImplementedException();
        }

        // ensure there are no invalid characters or mismatched rows
        public static ValidationResult ValidateFEN(string fen)
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

            result = ValidateHalfMoveClock(parts[4]);
            if (!result.IsValid) return result;

            result = ValidateFullMoveCounter(parts[5]);
            if (!result.IsValid) return result;

            return ValidationResult.Success();
        }

        private static bool isFENValidForGameboard(Gameboard gameboard, string fen)
        {
            throw new NotImplementedException();
        }

        public static bool IsEqualFEN(string fen1, string fen2)
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
            if (castlingAvailability.Contains("-"))
            {
                if (castlingAvailability == "-")
                    return ValidationResult.Success();
                else if (castlingAvailability.Any(c => c != '-' && !char.IsLetter(c)))
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

            // ensure the target is a valid square
            return ValidateAlgebraicNotation(enpassantTarget);
        }

        private static ValidationResult ValidateHalfMoveClock(string halfMoveClock)
        {
            try
            {
                int number = int.Parse(halfMoveClock);

                if (number < 0)
                    return ValidationResult.Fail($"The half-move clock value needs to be a non-negative integer: '{number}'.");

                return ValidationResult.Success();
            }
            catch (Exception)
            {
                return ValidationResult.Fail($"The half-move clock value is not a valid integer: '{halfMoveClock}'.");
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
    }
}
