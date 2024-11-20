using Chess.Classes;

namespace ChessLogic.Classes
{
    public static class GeneralUtils
    {
        public static bool Some<T>(this IEnumerable<T> source, Func<T, bool> predicate, int count)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            int matches = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    matches++;
                    if (matches >= count) return true;
                }
            }

            return false;
        }

        public static Dictionary<Piece, List<Action>> ToDictionary(this List<Action> legalActionList)
        {
            var legalActions = new Dictionary<Piece, List<Action>>();

            foreach (var action in legalActionList)
            {
                if (legalActions.ContainsKey(action.Piece))
                    legalActions[action.Piece].Add(action);
                else
                    legalActions[action.Piece] = new List<Action> { action };
            }

            return legalActions;
        }
    }
}
