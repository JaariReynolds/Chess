﻿using Chess.Classes.ConcretePieces;
using Chess.Types;
using ChessLogic.Classes;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[][] Board { get; set; }
        public TeamColour CurrentTeamColour { get; set; }
        public List<string> PreviousActions { get; set; }
        public Action? LastPerformedAction { get; set; }
        public int WhitePoints { get; set; }
        public int BlackPoints { get; set; }
        public TeamColour? CheckTeamColour { get; set; }
        public TeamColour? CheckmateTeamColour { get; set; }
        public bool IsStalemate { get; set; }
        public bool IsGameOver { get; set; }

        public Gameboard()
        {
            Board = ChessUtils.InitialiseBoard();
            PreviousActions = new List<string>();
            CurrentTeamColour = TeamColour.White;
            GameStateManager.Instance.Reset();
            CheckTeamColour = null;
            CheckmateTeamColour = null;
            IsStalemate = false;
            IsGameOver = false;
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
            PreviousActions = new List<string>(existingGameboard.PreviousActions);
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
            // update algebraic notation with check/mate symbol;
            action.AlgebraicNotation = AlgebraicNotationUtils.AddAlgebraicNotationSuffix(action.AlgebraicNotation, CheckTeamColour, CheckmateTeamColour);

            GameStateManager.Instance.UpdateLastPerformedAction(action);
            LastPerformedAction = action;
            PreviousActions.Add(action.AlgebraicNotation);
        }

        /// <summary>
        /// Calculates and returns a list of available actions for the provided team
        /// </summary>
        public List<Action> CalculateTeamActions(TeamColour teamColour)
        {
            var possibleActions = Board.GetAllPossibleActions(teamColour, true);
            var legalActions = this.GetLegalActions(possibleActions);
            AlgebraicNotationUtils.AlgebraicNotationAmbiguityResolution(legalActions);

            return legalActions;
        }

        public void ProcessTurn(Action action)
        {
            var originalAction = new Action(action);
            var originalTeamColour = CurrentTeamColour;

            PerformAction(action);
            CalculateGameStateStatus(originalTeamColour.GetOppositeTeam());
            FinaliseTurn(originalAction, originalTeamColour);
        }

        public void PerformAction(Action action)
        {
            if (action.Piece.TeamColour != CurrentTeamColour)
                throw new ArgumentException("Unable to perform the provided action as it is not currently their turn.");

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

            // SwapTurns required here in order for simulated actions to properly keep track of the current team
            // SwapTurns appropriately called again in FinaliseTurn() in order to stay in line with the real game 
            SwapTurns();
        }

        private void CalculateGameStateStatus(TeamColour teamColour)
        {
            var legalActions = CalculateTeamActions(teamColour);
            var isKingInCheck = Board.IsKingInCheck(teamColour);

            CheckTeamColour = isKingInCheck ? teamColour : null;
            CheckmateTeamColour = (legalActions.Count == 0 && CheckTeamColour == teamColour) ? teamColour : null;
            IsStalemate = legalActions.Count == 0 && CheckTeamColour == null;
            IsGameOver = CheckmateTeamColour != null || IsStalemate;
        }

        private void FinaliseTurn(Action originalAction, TeamColour originalTeamColour)
        {
            AddActionToHistory(originalAction);

            if (originalTeamColour == CurrentTeamColour)
                SwapTurns();
        }

        // console use only
        public static void ShowActions(List<Action> actions)
        {
            var index = 0;
            foreach (var action in actions)
            {
                Console.WriteLine($"   {index}. {action}");
                index++;
            }
        }

        // console use only
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
            Board.SetSquare(new Rook(TeamColour.White, "a8"));
            Board.SetSquare(new Rook(TeamColour.White, "g8"));
        }
    }
}
