namespace ChessLogic.Classes
{
    public struct ValidationResult
    {
        public bool IsValid { get; }
        public string ErrorReason { get; }

        public ValidationResult(bool isValid, string errorReason = "")
        {
            IsValid = isValid;
            ErrorReason = errorReason;
        }

        public static ValidationResult Success() => new ValidationResult(true);
        public static ValidationResult Fail(string reason) => new ValidationResult(false, reason);
    }
}
