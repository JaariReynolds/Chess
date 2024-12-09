using Chess.Classes;

namespace ChessLogic.Classes
{
    public static class ForsythEdwardsNotation
    {

        private static string validCharacters = "kKQqRrBbNnPp";

        public static string GenerateFEN(Gameboard gameboard)
        {
            throw new NotImplementedException();
        }

        public static Gameboard ParseFEN(string fen)
        {
            throw new NotImplementedException();
        }

        // ensure there are no invalid characters or mismatched rows
        private static ValidationResult ValidateFEN(string fen)
        {
            if (string.IsNullOrEmpty(fen))
                return ValidationResult.Fail("FEN string is null or empty.");


            string[] parts = fen.Split(" ");
            if (parts.Length != 6)
                return ValidationResult.Fail($"FEN string does not contain 6 sections. Sections provided: {parts.Length}.");

            // piece placement validation
            var piecePlacement = ValidatePiecePlacement(parts[0]);

            // active colour validation
            // castling availability validation
            // en passant target validation
            // half-move-clock validation
            // full-move counter validation


            //throw new NotImplementedException();
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
            string[] ranks = piecePlacement.Split("/");
            if (ranks.Length != 8)
                return ValidationResult.Fail($"Piece placement section does not contain 8 ranks. Ranks provided: {ranks.Length}. ");

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


    }
}
