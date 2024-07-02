using Chess.Classes;

namespace Chess.Classes
{
    public class Gameboard
    {
        public Piece[,] Board { get; private set; }
        public TeamColour CurrentTeam { get; private set; }
        public Dictionary<Piece, List<Action>> CurrentTeamActions { get; private set; }
        public Action? LastPerformedAction { get; private set; }

        public Gameboard()
        {
            Board = new Piece[8, 8];
            CurrentTeam = TeamColour.White;
            //InitialiseBoardState();
            CurrentTeamActions = new Dictionary<Piece, List<Action>>();
        }
        public void InitialiseBoardState()
        {
            Board[1, 3] = new Pawn(TeamColour.White, 1, 3);
            Board[0, 2] = new Pawn(TeamColour.Black, 0, 2);
            Board[1, 5] = new Pawn(TeamColour.Black, 1, 5);



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

                List<Action>? pieceActions;
                if (piece is Pawn)
                    pieceActions = piece.GetPotentialActions(Board, LastPerformedAction);
                else
                    pieceActions = piece.GetPotentialActions(Board, null);

                if (pieceActions.Count != 0)
                {
                    pieceActions = pieceActions.OrderBy(a => a.ToString()).ToList();
                    CurrentTeamActions[piece] = pieceActions;
                }
            }

            Console.WriteLine($"Actions for {CurrentTeam}");
            int selector = 1;

            foreach (var pair in CurrentTeamActions)
            {
                Console.WriteLine($"{selector}. {pair.Key}: ");
                selector++;
                foreach (var value in pair.Value)
                {
                    Console.WriteLine($"    {value}");
                }
                Console.WriteLine();
            }
        }

        public void SelectPiece()
        {
            int selected;
            bool validSelection;
            Console.WriteLine("Select a valid piece:");
            do
            {
                validSelection = int.TryParse(Console.ReadLine(), out selected) 
                    && selected <= CurrentTeamActions.Count 
                    && selected >= 1;

                if (!validSelection)
                    Console.WriteLine("invalid");
                
            } while (!validSelection);

            int counter = 1;
            Piece selectedPiece;
            foreach (var pair in CurrentTeamActions)
            {
                if (counter != selected)
                    break;

                selectedPiece = pair.Key;
            }
            
        }

        public void SelectAction(int actionIndex)
        {

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
