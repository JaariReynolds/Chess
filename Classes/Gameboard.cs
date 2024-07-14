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
            Board.SetPieceAt("a1", new Pawn(TeamColour.White, "a1"));
            Board.SetPieceAt("a4", new Pawn(TeamColour.White, "a4"));
            Board.SetPieceAt("h1", new Knight(TeamColour.Black, "h1"));
            Board[5, 4] = new Rook(TeamColour.Black, 5, 4);
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

        public void SetLastPerformedAction(Action action)
        {
            LastPerformedAction = action;
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
