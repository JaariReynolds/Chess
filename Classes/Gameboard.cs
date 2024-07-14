using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[,] Board { get; private set; }
        public TeamColour CurrentTeamColour { get; private set; }
        public List<Action> WhiteActions { get; private set; }
        public List<Action> BlackActions { get; private set; }
        public Action? LastPerformedAction { get; private set; }

        public Gameboard()
        {
            Board = new Piece[8, 8];
            CurrentTeamColour = TeamColour.White;
            //InitialiseBoardState();
            WhiteActions = new List<Action>();
            BlackActions = new List<Action>();
        }
        public void InitialiseBoardState()
        {
            Board[0, 0] = new King(TeamColour.White, 0, 0);
            Board[1, 1] = new Pawn(TeamColour.Black, 1, 1);
            //Board[3, 4] = new Pawn(TeamColour.Black, 3, 4);

            //Board[5, 1] = new Pawn(TeamColour.White, 5, 1);
            //Board[5, 3] = new Pawn(TeamColour.White, 5, 3); 




            //for (int y = 0; y < Board.GetLength(0); y++)
            //{
            //    Board[6, y] = new Pawn(TeamColour.White, 6, y);
            //    Board[1, y] = new Pawn(TeamColour.Black, 1, y);
            //}
        }

        public void SetTestBoard(int x, int y, Piece piece)
        {
            Board[x, y] = piece;
        }

        public void SwapTurns()
        {
            CurrentTeamColour = CurrentTeamColour == TeamColour.White ? TeamColour.Black : TeamColour.White;
            WhiteActions.Clear();
            BlackActions.Clear();
        }

        public Gameboard Clone()
        {
            Gameboard cloneBoard = new Gameboard();

            for (int row = 0; row < Board.GetLength(0); row++)
            {
                for (int col = 0; col < Board.GetLength(1); col++)
                {
                    if (Board[row, col] != null)
                    {
                        cloneBoard.Board[row, col] = Board[row, col].Clone();
                    }
                }
            }

            return cloneBoard;
        }

        public void CalculateTeamActions(TeamColour teamColour)
        {
            var actions = new List<Action>();

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

            if (teamColour == TeamColour.White)
                WhiteActions = actions;
            else
                BlackActions = actions;

            foreach (var action in actions)
            {
                Console.WriteLine(action);
            }
        }

        private void PerformAction(Piece piece, Action action)
        {
            switch (action.ActionType)
            {
                case ActionType.Move:
                    Board[action.Square.X, action.Square.Y] = piece;
                    Board[piece.Square.X, piece.Square.Y] = null!;
                    piece.MovePiece(action.Square.X, action.Square.Y);
                    break;
            }
        }

        public void SetLastPerformedAction(Action action)
        {
            LastPerformedAction = action;
        }

        public void DrawCurrentState()
        {
            Console.WriteLine("   | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 |");
            Console.WriteLine("   ---------------------------------");
            for (int x = 0; x < Board.GetLength(0); x++)
            {
                Console.Write($" {x} ");
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
