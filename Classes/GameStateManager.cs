namespace ChessLogic.Classes
{
    public class GameStateManager
    {
        private static GameStateManager? _instance;
        public static GameStateManager Instance => _instance ??= new GameStateManager();

        public Action? LastPerformedAction { get; private set; }

        public void UpdateLastPerformedAction(Action action)
        {
            LastPerformedAction = action;
        }
    }
}
