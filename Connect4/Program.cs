using System;

namespace Connect4
{
    class Controller
    {
        private string Winner { get; set; }
        private string GameMode { get; set; }
        private int PlayerTurn { get; set; }
        public Controller()
        {
            Console.WriteLine("For Human vs. AI enter 1");
            Console.WriteLine("For Human vs. Human enter 2");
            StartGame();
        }
        private void StartGame()
        {
            GameMode = Console.ReadLine();
            if (GameMode == "2")
            {
                Model Game = new Model();
                Player PlayerOne = new Human(1);
                Player PlayerTwo = new Human(2);
                Winner = "";
                while (Winner.Length == 0)
                {
                    Game.DrawBoard();
                    if (PlayerTurn == 41) //If all 42 spots have been filled then it is a tie
                    {
                        Winner = "No One";
                        break;
                    }
                    if (PlayerTurn % 2 == 0)                        //Every even turn is player one's turn
                    {
                        
                        if (Game.SetMove('X', PlayerOne.MakeMove())) //Was player able to place here?
                        {
                            Winner = Game.CheckWinner('X');         //Check if there is winner of X
                            PlayerTurn++;                           //Increase turn counter
                        }
                        else                                        //If not, run this again
                        {
                            Console.Write("Cannot place");
                            System.Threading.Thread.Sleep(2000);
                            continue;
                        }
                    }
                    else                                            //Every odd turn is player two's turn
                    {
                        if (Game.SetMove('O', PlayerTwo.MakeMove()))
                        {
                            Winner = Game.CheckWinner('O');
                            PlayerTurn++;
                        }
                        else
                        {
                            Console.Write("Cannot place");
                            System.Threading.Thread.Sleep(2000);
                            continue;
                        }
                    }
                }
                Console.Clear();            //Clear board
                if (Winner == "X")          //If winner is found then set it to Player One instead of the token
                {
                    Winner = PlayerOne.GetName();
                }
                else if (Winner == "O")     //If token is O then set it to player two
                {
                    Winner = PlayerTwo.GetName();
                }
                Console.WriteLine("Winner is {0}", Winner);
            }
            else if (GameMode == "1")
            {
                Model Game = new Model();
                Player PlayerOne = new Human(1);
                Computer Computer = new Computer();
                Winner = "";
                while (Winner.Length == 0)
                {
                    Game.DrawBoard();
                    if (PlayerTurn == 41) //If all 42 spots have been filled then it is a tie
                    {
                        Winner = "No One";
                        break;
                    }
                    if (PlayerTurn % 2 == 0)                            //Player one's turn
                    {
                        if (Game.SetMove('X', PlayerOne.MakeMove()))
                        {
                            Winner = Game.CheckWinner('X');
                            PlayerTurn++;
                        }
                        else
                        {
                            Console.Write("Cannot place");
                            System.Threading.Thread.Sleep(2000);
                            continue;
                        }
                    }
                    else                                                //Computer's turn
                    {
                        if (Game.SetMove('O', Computer.MakeMove(Game.WinnerInFuture())))
                        {
                            //Place where anyone has a potential to win next turn
                            Winner = Game.CheckWinner('O');
                            PlayerTurn++;
                        }
                    }
                }
                Console.Clear();
                if (Winner == "X")
                {
                    Winner = PlayerOne.GetName();
                }
                else if (Winner == "O")
                {
                    Winner = "Computer";
                }
                Console.WriteLine("Winner is {0}", Winner);
            }
            else
            {
                Console.WriteLine("Invalid gamemode");
                StartGame();
            }
        }
    }
    class Model
    {
        private char[,] GameBoard { get; set; }
        public Model()
        {
            GameBoard = new char[,] { 
                {'#','#','#','#','#','#','1'},  //Left most value is placed at the top of the board
                {'#','#','#','#','#','#','2'},
                {'#','#','#','#','#','#','3'},
                {'#','#','#','#','#','#','4'},
                {'#','#','#','#','#','#','5'},
                {'#','#','#','#','#','#','6'},
                {'#','#','#','#','#','#','7'}
            };
        }
        public void DrawBoard()
        {
            Console.Clear();                                //Clear board
            for (int y = 0; y < 7; y++)
            {
                Console.Write("|");                         //First draw left border
                for (int x = 0; x < 7; x++)
                {
                    Console.Write("{0,2}",GameBoard[x,y]);  //Draw the current value at that pos with formatted text
                }
                Console.Write(" |");                        //Draw right border
                Console.WriteLine();
            }
        }
        public bool SetMove(char playerToken, int column)       //Place the recieved player token at recieved column
        {
            for (int y = 5; y >= 0 && y < 6; y--)       //Loop from lowest row in column
            {
                if (GameBoard[column, y] == '#')        //Place at lowest available row in column
                {
                    GameBoard[column, y] = playerToken;
                    return true;
                }
                if (GameBoard[column, y] != '#' && y == 0)  //If unable to place tell user unable to place
                {
                    return false;
                }
            }
            return false;
        }
        public int WinnerInFuture()
        {
            char[,] TestBoard = GameBoard;
            Random random = new Random();
            if (random.Next(0, 100) > 30)       //To create a bias or difficulty
            {
                for (int playerToken = 0; playerToken < 2; playerToken++)
                {
                    char Token;
                    if (playerToken == 0)
                    {
                        Token = 'X';
                    }
                    else
                    {
                        Token = 'O';
                    }
                    for (int x = 0; x < 7; x++)
                    {
                        for (int y = 5; y >= 0 && y < 6; y--)       //Loop from lowest row in column
                        {
                            if (TestBoard[x, y] == '#')        //Place at lowest available row in column
                            {
                                TestBoard[x, y] = Token;
                                if (CheckWinner(Token) == "")
                                {
                                    TestBoard[x, y] = '#';
                                }
                                else
                                {
                                    TestBoard[x, y] = '#';
                                    return x;
                                }
                            }
                        }
                    }
                }
            }
            return 8; //Because 8 is not a valid column it will trigger the computer to generate a random number between 1-7 instead
        }
        public string CheckWinner(char playerToken)
        {
            for (int y = 0; y < 6; y++)         //Limit inner scope to 0-5 (inclusive)
            {
                for (int x = 0; x < 7; x++)     //Limit outer scope to 0-6 (inclusive)
                {
                    if (GameBoard[x,y] != playerToken)
                    {
                        continue; //Ignore Empty and Enemy tiles
                    }
                    if (
                        x + 3 < 7 &&                            //Is there enough space for horizontal connect?
                        playerToken == GameBoard[x + 1, y] &&   //Is the tile right next to selected tile the same?
                        playerToken == GameBoard[x + 2, y] &&   //Is 2 tiles right next to selected the same?
                        playerToken == GameBoard[x + 3, y]      //Is 3 tiles right next to selected the same?
                        ) { return playerToken.ToString(); } 
                    if (
                        y + 3 < 6 &&                            //Is there enough space for vertical connect?
                        playerToken == GameBoard[x, y + 1] &&   //Is the tile above the selected tile the same?
                        playerToken == GameBoard[x, y + 2] &&   //Is 2 tiles above the selected tile the same?
                        playerToken == GameBoard[x, y + 3]      //Is 3 tiles above the selected tile the same?
                        ) { return playerToken.ToString(); }
                    if (
                        x - 3 >= 0 &&                           //Is there enough space?
                        playerToken == GameBoard[x - 1, y] &&   //Is the tile left of selected same?
                        playerToken == GameBoard[x - 2, y] &&   //Is 2 tile left of selected same?
                        playerToken == GameBoard[x - 3, y]      //Is 3 tile left of selected same?
                        ) { return playerToken.ToString(); }
                    if (
                        y - 3 >= 0 &&                           //Is there enough space?
                        playerToken == GameBoard[x, y - 1] &&   //Is the tile down of selected same?
                        playerToken == GameBoard[x, y - 2] &&   //Is 2 tile down of selected same?
                        playerToken == GameBoard[x, y - 3]      //Is 3 tile down of selected same?
                        ) { return playerToken.ToString(); }
                    if (
                        x - 3 >= 0 &&                               //Is there enough horizontal space?
                        y - 3 >= 0 &&                               //Is there enough vertical space?
                        playerToken == GameBoard[x - 1, y - 1] &&   //Is the tile diagonally left the same?
                        playerToken == GameBoard[x - 2, y - 2] &&   //Is the tile 2 diagonally left the same?
                        playerToken == GameBoard[x - 3, y - 3]      //Is the tile 3 diagonally left the same?
                        ) { return playerToken.ToString(); }
                    if (
                        x + 3 < 7 &&                                //Is there enough horizontal space?
                        y - 3 >= 0 &&                               //Is there enough vertical space?
                        playerToken == GameBoard[x + 1, y - 1] &&   //Is the tile diagonally left the same?
                        playerToken == GameBoard[x + 2, y - 2] &&   //Is the tile 2 diagonally left the same?
                        playerToken == GameBoard[x + 3, y - 3]      //Is the tile 3 diagonally left the same?
                        ) { return playerToken.ToString(); }
                    if (
                        x - 3 >= 0 &&                               //Is there enough horizontal space?
                        y + 3 < 6 &&                                //Is there enough vertical space?
                        playerToken == GameBoard[x - 1, y + 1] &&   //Is the tile diagonally left the same?
                        playerToken == GameBoard[x - 2, y + 2] &&   //Is the tile 2 diagonally left the same?
                        playerToken == GameBoard[x - 3, y + 3]      //Is the tile 3 diagonally left the same?
                        ) { return playerToken.ToString(); }
                    if (
                        x + 3 < 7 &&                                //Is there enough horizontal space?
                        y + 3 < 6 &&                                //Is there enough vertical space?
                        playerToken == GameBoard[x + 1, y + 1] &&   //Is the tile diagonally right the same?
                        playerToken == GameBoard[x + 2, y + 2] &&   //Is the tile 2 diagonally right the same?
                        playerToken == GameBoard[x + 3, y + 3]      //Is the tile 3 diagonally right the same?
                        ) { return playerToken.ToString(); }
                }
            }
            return "";
        }
    }
    abstract class  Player
    {

        public abstract int MakeMove();
        public abstract string GetName();
    }
    class Human : Player
    {
        protected string Name { get; set; }
        public Human(int pos)
        {
            ClearLine();
            Console.Write("(Player {0}) What is your name?: ", pos);
            Name = Console.ReadLine();
        }
        public override string GetName()
        {
            return Name;
        }
        public override int MakeMove()
        {
            int ColumnMove;
            string IsValid;
            while (true)
            { 
                Console.Write("({0}) Select which column you want to move to: ", this.Name);
                IsValid = Console.ReadLine();
                ClearLine();
                if (IsValid != "") //Check if input is not empty
                {
                    ColumnMove = int.Parse(IsValid);
                    if (ColumnMove > 0 && ColumnMove <= 7) //Check if input is between 1 - 7 (inclusive)
                    {
                        return ColumnMove - 1;      //Shifted down one because index starts at 0 instead of 1
                    }
                    else
                    {
                        Console.Write("(Invalid, Try again) ");     //Add Invalid notification
                    }
                }
                else
                {
                    Console.Write("(Invalid, Try again) ");     //Add Invalid notification
                }
            }
        }
        public void ClearLine()
        {
            Console.SetCursorPosition(Console.BufferWidth - 1, Console.CursorTop - 1);   //Set the cursor to the end of the previous line
            do { Console.Write("\b \b"); } while (Console.CursorLeft > 0);              //Remove characters until cursor is at beginning
        }
    }
    class Computer
    {
        Random random = new Random();
        protected string Name { get; set; }
        public int MakeMove(int i)
        {
            if (i == 8)
            {
                return random.Next(1,7);
            }
            else
            {
                return i;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Controller ConnectGame = new Controller();
        }
    }
}
