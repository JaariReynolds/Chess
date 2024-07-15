namespace Chess.Types
{
    public enum TeamColour
    {
        White,
        Black
    }

    public static class TeamColourExtensions
    {
        public static TeamColour GetOppositeTeam(this TeamColour teamColour)
        {
            return teamColour == TeamColour.White ? TeamColour.Black : TeamColour.White;
        }
    }
}
