using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[][] Board { get; set; }
        public TeamColour CurrentTeamColour { get; set; }
        public List<Action> PreviousActions { get; set; }
        public int WhitePoints { get; set; }
        public int BlackPoints { get; set; }
        public CheckStatus CheckedTeamColour { get; set; } = CheckStatus.None;
        public CheckStatus CheckmateTeamColour { get; set; } = CheckStatus.None;

        public Gameboard()
        {
            Board = ChessUtils.InitialiseBoard();
            PreviousActions = new List<Action>();
            CurrentTeamColour = TeamColour.White;
        }

        /// <summary>
        /// Create a (deep) copy of a Gameboard for simulation purposes
        /// </summary>
        public Gameboard(Gameboard existingGameboard)
        {
            Board = ChessUtils.InitialiseBoard();
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                    if (existingGameboard.Board[row][col] != null)
                        Board[row][col] = existingGameboard.Board[row][col].Clone();

            CurrentTeamColour = existingGameboard.CurrentTeamColour;
            WhitePoints = existingGameboard.WhitePoints;
            BlackPoints = existingGameboard.BlackPoints;
            PreviousActions = new List<Action>(existingGameboard.PreviousActions);
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

            for (int col = 0; col < 8; col++)
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

        public void SwapTurns()
        {
            CurrentTeamColour = CurrentTeamColour.GetOppositeTeam();
        }

        public void AddActionToHistory(Action action)
        {
            PreviousActions.Add(action);
        }

        /// <summary>
        /// Calculates and returns a dictionary of available actions for the provided team, while also setting on the gameboard if there is a check or mate.
        /// </summary>

        public Dictionary<Piece, List<Action>> CalculateTeamActions(TeamColour teamColour)
        {
            var lastPerformedAction = PreviousActions.Count == 0 ? null : PreviousActions[^1];
            var parsedCheckColour = (CheckStatus)Enum.Parse(typeof(CheckStatus), teamColour.ToString());

            var isKingInCheck = Board.IsKingInCheck(teamColour, lastPerformedAction);
            var possibleActions = Board.GetAllPossibleActions(teamColour, lastPerformedAction, true);
            var legalActions = this.GetLegalActionsDictionary(possibleActions);

            CheckedTeamColour = isKingInCheck ? parsedCheckColour : CheckStatus.None;
            CheckmateTeamColour = legalActions.Count == 0 ? parsedCheckColour : CheckStatus.None;

            return legalActions;
        }

        /// <summary>
        /// (For Chessbot use only)
        /// Calculates and returns a list of available actions for the provided team, while also setting on the gameboard if there is a check or mate.
        /// </summary>
        public List<Action> CalculateTeamActionsList(TeamColour teamColour)
        {
            var lastPerformedAction = PreviousActions.Count == 0 ? null : PreviousActions[^1];
            var parsedCheckColour = (CheckStatus)Enum.Parse(typeof(CheckStatus), teamColour.ToString());

            var isKingInCheck = Board.IsKingInCheck(teamColour, lastPerformedAction);
            var possibleActions = Board.GetAllPossibleActions(teamColour, lastPerformedAction, true);
            var legalActions = this.GetLegalActionsList(possibleActions);

            CheckedTeamColour = isKingInCheck ? parsedCheckColour : CheckStatus.None;
            CheckmateTeamColour = legalActions.Count == 0 ? parsedCheckColour : CheckStatus.None;

            return legalActions;
        }

        public void PerformAction(Action action)
        {
            if (action.Piece.TeamColour != CurrentTeamColour)
                throw new ArgumentException("Unable to perform the provided action as it is not currently their turn.");

            var originalAction = new Action(action); // copy of action to store the original Square/HasMoved of the piece 

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
            AddActionToHistory(originalAction);

            CheckedTeamColour = CheckStatus.None; // set to None as it will be recalculated on the opposing team's moves

            SwapTurns();
        }

        // console use only
        public static void ShowActions(Dictionary<Piece, List<Action>> actions)
        {
            var index = 0;
            foreach (var kvp in actions)
            {
                foreach (var action in kvp.Value)
                {
                    Console.WriteLine($"   {index}. {action}");
                    index++;
                }
            }
        }

        // console use only
        public Action SelectAction(Dictionary<Piece, List<Action>> actions)
        {
            int selectedActionIndex;
            bool validSelection;

            Console.WriteLine("Select action:");
            ShowActions(actions);

            var listedActions = actions.SelectMany(kvp => kvp.Value).ToList();

            do
                validSelection = int.TryParse(Console.ReadLine(), out selectedActionIndex) && selectedActionIndex < listedActions.Count;
            while (!validSelection);

            return actions.SelectMany(kvp => kvp.Value).ElementAt(selectedActionIndex);
        }

        // console use only
        public void DrawCurrentState()
        {
            Console.WriteLine("   | a | b | c | d | e | f | g | h |");
            Console.WriteLine("   ---------------------------------");
            for (int i = 0; i < Board.Length; i++)
            {
                Console.Write($" {8 - i} ");
                for (int j = 0; j < Board[i].Length; j++)
                {
                    if (j == 0) Console.Write("|");
                    if (Board[i][j] == null)
                    {
                        Console.Write("   |");
                    }
                    else
                    {
                        Board[i][j].Draw();
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
            }
        }


        // for console testing only 
        public void InitialiseTestBoardState()
        {
            Board.SetSquare(new King(TeamColour.White, "e1"));
            Board.SetSquare(new Rook(TeamColour.White, "h1"));
            Board.SetSquare(new King(TeamColour.Black, "e8"));
            Board.SetSquare(new Rook(TeamColour.Black, "h8"));
        }
    }
}
