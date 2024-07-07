using Chess.Classes;
using Chess.Classes.ConcretePieces;
using Chess.Types;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[,] Board { get; private set; }
        public TeamColour CurrentTeam { get; private set; }
        public  List<Action> CurrentTeamActions { get; private set; }
        public Action? LastPerformedAction { get; private set; }

        public Gameboard()
        {
            Board = new Piece[8, 8];
            CurrentTeam = TeamColour.White;
            //InitialiseBoardState();
            CurrentTeamActions = new List<Action>();
        }
        public void InitialiseBoardState()
        {
            Board[3, 3] = new Bishop(TeamColour.White, 3, 3);
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
            CurrentTeam = CurrentTeam == TeamColour.White ? TeamColour.Black : TeamColour.White;
        }

        public void CalculateCurrentTeamActions()
        {
            CurrentTeamActions.Clear();
            foreach (var piece in Board)
            {
                if (piece == null) continue;
                if (piece.TeamColour != CurrentTeam) continue;

                if (piece is Pawn)
                    CurrentTeamActions.AddRange(piece.GetPotentialActions(Board, LastPerformedAction));
                else
                    CurrentTeamActions.AddRange(piece.GetPotentialActions(Board, null));
            }

            CurrentTeamActions = CurrentTeamActions.OrderBy(a => a.ToString()).ToList();

            foreach (var action in CurrentTeamActions)
            {
                Console.WriteLine(action);
            }
        }

        private void PerformAction(Piece piece, Action action)
        {
            switch (action.ActionType)
            {
                case ActionType.Move:
                    Board[action.ActionX, action.ActionY] = piece;
                    Board[piece.PositionX, piece.PositionY] = null!;
                    piece.MovePiece(action.ActionX, action.ActionY);
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
