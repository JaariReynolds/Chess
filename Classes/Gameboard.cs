using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[,] Board { get; private set; }
        public TeamColour CurrentTeamColour { get; set; }

        public Action? LastPerformedAction { get; set; }
        public int WhitePoints { get; set; }
        public int BlackPoints { get; set; }
        public bool IgnoreKing { get; set; } // used for testing only

        public Gameboard()
        {
            Board = new Piece[8, 8];
            CurrentTeamColour = TeamColour.White;
            //InitialiseBoardState();
        }

        /// <summary>
        /// Create a copy of a Gameboard for simulation purposes
        /// </summary>
        public Gameboard(Gameboard copiedGameboard)
        {
            var clonedBoard = new Piece[8, 8];
            for (int row = 0; row < copiedGameboard.Board.GetLength(0); row++)
                for (int col = 0; col < copiedGameboard.Board.GetLength(1); col++)
                    if (copiedGameboard.Board[row, col] != null)
                        clonedBoard[row, col] = copiedGameboard.Board[row, col].Clone();

            CurrentTeamColour = copiedGameboard.CurrentTeamColour;
            WhitePoints = copiedGameboard.WhitePoints;
            BlackPoints = copiedGameboard.BlackPoints;
            LastPerformedAction = copiedGameboard.LastPerformedAction;
        }

        public void InitialiseBoardState()
        {
            Board.SetPieceAt("a2", new Pawn(TeamColour.White, "a2"));
            Board.SetPieceAt("h1", new Knight(TeamColour.Black, "h1"));
            Board[5, 4] = new Rook(TeamColour.Black, 5, 4);
        }

        public void SetTestBoard(int x, int y, Piece piece)
        {
            Board[x, y] = piece;
        }

        public void SwapTurns()
        {
            CurrentTeamColour = CurrentTeamColour.GetOppositeTeam();
            Console.WriteLine("current team colour is " + CurrentTeamColour);
        }

        public List<Action> CalculateTeamActions(TeamColour teamColour)
        {
            var actions = new List<Action>();
            var checkingPieces = new List<Piece>();

            // determine if King is in check before calculating possible moves 
            if (!IgnoreKing)
                if (this.IsKingInCheck(CurrentTeamColour))
                    checkingPieces = this.GetCheckingPieces(CurrentTeamColour);

            // calculate potential moves of all pieces of the current team
            foreach (var piece in Board)
            {
                if (piece == null) continue;
                if (piece.TeamColour != teamColour) continue;

                if (piece is Pawn)
                    actions.AddRange(piece.GetPotentialActions(Board, LastPerformedAction));
                else
                    actions.AddRange(piece.GetPotentialActions(Board, null));
            }

            actions = actions.OrderBy(a => a.ToString()).ToList();

            return actions;
        }

        public void ShowActions(List<Action> actions)
        {
            for (int i = 0; i < actions.Count; i++)
                Console.WriteLine($"   {i}. {actions[i]}");
        }

        public Action SelectAction(List<Action> actions)
        {
            int selectedActionIndex;
            bool validSelection;

            Console.WriteLine("Select action:");
            ShowActions(actions);

            do
                validSelection = int.TryParse(Console.ReadLine(), out selectedActionIndex) && selectedActionIndex < actions.Count;
            while (!validSelection);

            return actions[selectedActionIndex];
        }

        public void PerformAction(Action action)
        {
            switch (action.ActionType)
            {
                case ActionType.Move:
                case ActionType.PawnDoubleMove:
                    this.Move(action);
                    break;

                case ActionType.Capture:
                    this.Capture(action);
                    break;

                default:
                    throw new NotImplementedException($"{action.ActionType} not yet implemented");
            }

            LastPerformedAction = action;
            SwapTurns();
        }

        public void DrawCurrentState()
        {
            Console.WriteLine("   | a | b | c | d | e | f | g | h |");
            Console.WriteLine("   ---------------------------------");
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                Console.Write($" {8 - x} ");
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    if (y == 0) Console.Write("|");
                    if (Board[x, y] == null)
                    {
                        Console.Write("   |");
                    }
                    else
                    {
                        Board[x, y].Draw();
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
