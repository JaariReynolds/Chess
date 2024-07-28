using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[,] Board { get; private set; }
        public TeamColour CurrentTeamColour { get; private set; }
        public List<Action> PreviousActions { get; private set; }
        public int WhitePoints { get; set; }
        public int BlackPoints { get; set; }
        public bool IgnoreKing { get; set; } // used for testing only

        public Gameboard()
        {
            Board = new Piece[8, 8];
            PreviousActions = new List<Action>();
            CurrentTeamColour = TeamColour.White;
            //InitialiseStandardBoardState();
        }

        /// <summary>
        /// Create a copy of a Gameboard for simulation purposes
        /// </summary>
        public Gameboard(Gameboard existingGameboard)
        {
            Board = new Piece[8, 8];
            for (int row = 0; row < existingGameboard.Board.GetLength(0); row++)
                for (int col = 0; col < existingGameboard.Board.GetLength(1); col++)
                    if (existingGameboard.Board[row, col] != null)
                        Board[row, col] = existingGameboard.Board[row, col].Clone();

            CurrentTeamColour = existingGameboard.CurrentTeamColour;
            WhitePoints = existingGameboard.WhitePoints;
            BlackPoints = existingGameboard.BlackPoints;
            PreviousActions = existingGameboard.PreviousActions;
        }

        public void InitialiseTestBoardState()
        {
            Board.SetSquare(new King(TeamColour.White, "e1"));
            Board.SetSquare(new Rook(TeamColour.White, "h1"));
            Board.SetSquare(new Rook(TeamColour.White, "a1"));
            Board.SetSquare(new Pawn(TeamColour.Black, "d7"));
        }

        public void InitialiseStandardBoardState()
        {
            Board.SetSquare(new Rook(TeamColour.Black, "a8"));
            Board.SetSquare(new Knight(TeamColour.Black, "b8"));
            Board.SetSquare(new Bishop(TeamColour.Black, "c8"));
            Board.SetSquare(new Queen(TeamColour.Black, "d8"));
            Board.SetSquare(new King(TeamColour.Black, "e8"));
            Board.SetSquare(new Bishop(TeamColour.Black, "f8"));
            Board.SetSquare(new Knight(TeamColour.Black, "g8"));
            Board.SetSquare(new Rook(TeamColour.Black, "h8"));

            for (int col = 0; col < Board.GetLength(1); col++)
            {
                Board.SetSquare(new Pawn(TeamColour.Black, 1, col));
                Board.SetSquare(new Pawn(TeamColour.White, 6, col));
            }

            Board.SetSquare(new Rook(TeamColour.White, "a1"));
            Board.SetSquare(new Knight(TeamColour.White, "b1"));
            Board.SetSquare(new Bishop(TeamColour.White, "c1"));
            Board.SetSquare(new Queen(TeamColour.White, "d1"));
            Board.SetSquare(new King(TeamColour.White, "e1"));
            Board.SetSquare(new Bishop(TeamColour.White, "f1"));
            Board.SetSquare(new Knight(TeamColour.White, "g1"));
            Board.SetSquare(new Rook(TeamColour.White, "h1"));
        }

        public void SetTestBoard(int x, int y, Piece piece)
        {
            Board[x, y] = piece;
        }

        public void SwapTurns()
        {
            CurrentTeamColour = CurrentTeamColour.GetOppositeTeam();
        }

        public void AddActionToHistory(Action action)
        {
            PreviousActions.Add(action);
        }

        public List<Action> CalculateTeamActions(TeamColour teamColour)
        {
            var lastPerformedAction = PreviousActions.Count == 0 ? null : PreviousActions[^1];
            var possibleActions = Board.GetAllPossibleActions(teamColour, lastPerformedAction);
            var legalActions = this.GetLegalActions(possibleActions);
            return legalActions.OrderBy(a => a.ToString()).ToList();
        }

        public static void ShowActions(List<Action> actions)
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

                case ActionType.PawnPromoteKnight:
                case ActionType.PawnPromoteBishop:
                case ActionType.PawnPromoteRook:
                case ActionType.PawnPromoteQueen:
                    this.Promote(action);
                    break;

                case ActionType.PawnEnPassant:
                    this.EnPassant(action);
                    break;

                case ActionType.KingsideCastle:
                    this.KingsideCastle(action);
                    break;

                case ActionType.QueensideCastle:
                    this.QueensideCastle(action);
                    break;

                default:
                    throw new NotImplementedException($"{action.ActionType} not yet implemented");
            }

            action.Piece.HasMoved = true;
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
