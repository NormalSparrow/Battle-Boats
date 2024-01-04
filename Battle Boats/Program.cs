using System;
using System.Linq.Expressions;
using System.Timers;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.IO;
enum tile
{
    hit,
    miss,
    empty,
    boat,
}

class Program
{
    static async Task Main()
    {
        tile[,] playerGrid = new tile[8, 8];
        tile[,] playerView = new tile[8, 8];
        tile[,] computerGrid = new tile[8, 8];

        string playerName = Menu.playername();
        Console.Clear();
        string input = Menu.menuOptions();

        Menu.MenuChoice(playerGrid, computerGrid, playerName, input);
        Game.loop( playerGrid, playerName, computerGrid);

    }
class Menu
{
    public static string playername()
    {
        Console.WriteLine("whats your name player?");
        string playerName = Console.ReadLine();
        return playerName;
    }

    public static string menuOptions()
    {
            Console.Clear();
        Console.WriteLine("helloooo welcome to the menu \n 1. new game \n 2. exit game \n 3. instructions\n 4. load game");
        Console.Write("Option: ");
        return Console.ReadLine();
    }

    public static async void MenuChoice(tile[,] playerGrid, tile[,] computerGrid,  string playerName, string input)
    {
            switch (input)
            {
                case "new game":
                    Game.PlayNewGame(playerGrid, computerGrid, playerName);
                    break;
                case "exit game":
                    ExitGame();
                    await Task.Delay(4000);
                    break;
                case "instructions":
                    Instructions();
                    Console.ReadLine();
                    break;
                case "load game":
                    Load.LoadPlayerName();
                    playerGrid = Load.LoadPlayerGrid();
                    computerGrid = Load.LoadComputerGrid();
                    Game.loop( playerGrid,  playerName,  computerGrid);

                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Menu.menuOptions();
                    Menu.MenuChoice(playerGrid, computerGrid, playerName, Console.ReadLine());
                    break;

            }
    }
    

    public static async void ExitGame()
    {

        Console.Clear();
        Console.Write("Exiting game");

        await Task.Delay(1000);
        Console.Write(".");
        await Task.Delay(1000);
        Console.Write(".");
        await Task.Delay(1000);
        Console.WriteLine(".");
        await Task.Delay(1000);
        System.Environment.Exit(0);
    }
    public static void Instructions()
        {
            Console.WriteLine("Welcome to Battleship!");
            Console.WriteLine("Rules:");

            DisplayRule("1. The game is played on a 10x10 grid.");
            DisplayRule("2. Each player has a fleet of boats to place on the grid.");
            DisplayRule("3. The fleet consists of ships of different sizes: Carrier (5), Battleship (4), Cruiser (3), Submarine (3), and Destroyer (2).");
            DisplayRule("4. Players take turns to place their boats on the grid.");
            DisplayRule("5. Boats can be placed vertically or horizontally, but not diagonally.");
            DisplayRule("6. Boats cannot overlap with each other on the grid.");
            DisplayRule("7. After placing the boats, players take turns to guess the coordinates of the opponent's Boats.");
            DisplayRule("8. The grid is marked with 'X' for a hit and 'O' for a miss.");
            DisplayRule("9. The game continues until one player sinks all the opponent's Boats.");

            Console.WriteLine("\nLet the battle begin!");

        }
        static void DisplayRule(string rule)
        {
            Console.WriteLine($"- {rule}");
        }
    }
class Save
    {
        public static void ToFile(string playerName, tile[,] playerGrid, tile[,] computerGrid)
        {
            string filename = @"./saved game.txt";
            using (StreamWriter sw = new StreamWriter(filename))
            {
            
                sw.WriteLine(playerName);

                sw.WriteLine(playerGrid.GetLength(0));
                sw.WriteLine(playerGrid.GetLength(1));

              
                for (int i = 0; i < playerGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < playerGrid.GetLength(1); j++)
                    {
                        sw.Write((int)playerGrid[i, j] + " ");
                    }
                    sw.WriteLine();
                }

               
                sw.WriteLine(computerGrid.GetLength(0));
                sw.WriteLine(computerGrid.GetLength(1));

             
                for (int i = 0; i < computerGrid.GetLength(0); i++)
                {
                    for (int j = 0; j < computerGrid.GetLength(1); j++)
                    {
                        sw.Write((int)computerGrid[i, j] + " ");
                    }
                    sw.WriteLine();
                    
                }
               
            }
        }
    }
    class Load
    {
        public static string LoadPlayerName()
        {
            string playerName = "";
            string filename = @"./saved game.txt";

            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    try
                    {
                        playerName = sr.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error reading player name from file: " + ex.Message);
                    }
                }
            }

            return playerName;
        }

        public static tile[,] LoadGridFromFile(StreamReader sr)
        {
            tile[,] grid = new tile[0, 0];

            try
            {
                grid = ReadGridFromFile(sr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading grid from file: " + ex.Message);
            }

            return grid;
        }

        public static tile[,] LoadPlayerGrid()
        {
            string filename = @"./saved game.txt";
            tile[,] playerGrid = new tile[0, 0];

            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    // Skip the player name line
                    sr.ReadLine();

                    playerGrid = LoadGridFromFile(sr);
                }
            }

            return playerGrid;
        }

        public static tile[,] LoadComputerGrid()
        {
            string filename = @"./saved game.txt";
            tile[,] computerGrid = new tile[0, 0];

            if (File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    for (var i = 0; i < 11; i++)
                    {
                        var pants = sr.ReadLine();
                        //console.WriteLine(sr.ReadLine());
                    }

                    computerGrid = LoadGridFromFile(sr);
                }
            }

            return computerGrid;
        }

        public static tile[,] ReadGridFromFile(StreamReader sr)
        {
            try
            {
                int rows = int.Parse(sr.ReadLine());
                int cols = int.Parse(sr.ReadLine());

                tile[,] grid = new tile[rows, cols];

                for (int i = 0; i < rows; i++)
                {
                    string[] rowValues = sr.ReadLine().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int j = 0; j < cols; j++)
                    {
                        if (int.TryParse(rowValues[j], out int cellValue))
                        {
                            grid[i, j] = (tile)cellValue;
                        }
                        else
                        {
                            Console.WriteLine($"Error: Invalid grid cell value at row {i}, column {j}. Value: {rowValues[j]}");
                            // You might want to set a default value or handle this case differently.
                            grid[i, j] = tile.empty;
                        }
                    }
                }

                return grid;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading grid from file: " + ex.Message);
                return new tile[0, 0]; // Return an empty grid in case of an error.
            }
        }
    }
    class Game
{
    public static void loop( tile[,]playerGrid, string playerName, tile[,] computerGrid)
    {
            bool turn;
            bool win = false;
            while (win == false)
            {

                Console.Clear();
                Display.DisplayGrid("Player Grid", playerGrid);
                turn = true;
                Player.PlayerTurn(playerName, playerGrid, turn, computerGrid);
                Game.checkWin(turn, playerGrid, computerGrid);
                Console.ReadKey();
                Console.Clear();
                Display.DisplayGrid("Player Grid:", playerGrid);

                turn = false;
                Computer.computerTurn(playerGrid, computerGrid, turn);
                Game.checkWin(turn, playerGrid, computerGrid);
            };
    }

    public static void checkWin(bool turn, tile[,] playerGrid, tile[,]computerGrid)
    {
       if (turn = true)
        {
            if (!computerGrid.Cast<tile>().Any(cell => cell == tile.boat))
            {
                  Console.WriteLine("You win! Congratulations!");
                  System.Environment.Exit(0);
            }
            
        }
        else
        {
            if (turn = false)
            {
                if (playerGrid.Cast<tile>().Any())
                {
                    Console.WriteLine("Computer Wins!");
                    System.Environment.Exit(0);
                }
            }
        }
    }

        public static void PlayNewGame(tile[,] playerGrid, tile[,] computerGrid, string playerName)
        {
            Create.InitializeGrid(playerGrid);
            Create.InitializeGrid(computerGrid);

            // Create separate hit or miss grids for player and computer

           
            Create.PlaceBoats($"{playerName}", playerGrid);
            Create.PlaceRandomBoats(computerGrid);
            // Assign the separate hit or miss grid
           
        }
}

    class Display
    {
        public static void DisplayGrid(string gridName, tile[,] grid)
        {
            Console.WriteLine($"{gridName}\n");


            Console.Write("  ");
            for (int col = 0; col < grid.GetLength(1); col++)
            {
                Console.Write($"{col + 1} ");
            }
            Console.WriteLine();


            for (int row = 0; row < grid.GetLength(0); row++)
            {
                Console.Write($"{row + 1} ");
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    switch (grid[row, col])
                    {
                        case tile.empty:
                            Console.Write("~ ");
                            grid[row, col] = tile.empty;
                            break;
                        case tile.hit:
                            Console.Write("X ");
                            grid[row, col] = tile.hit;
                            break;
                        case tile.miss:
                            Console.Write("O ");
                            grid[row, col] = tile.miss;
                            break;
                        case tile.boat:
                            Console.Write("B ");
                            grid[row, col] = tile.boat;
                            break;


                    }
                }

                Console.WriteLine();
            }
            Console.WriteLine("press enter to continue");
            Console.ReadLine();
            Console.Clear();
        }
        public static void PlayerViewGrid(tile[,] playerGrid, tile[,] playerView)
        {
            Console.WriteLine("Player View Grid\n");

            Console.Write("  ");
            for (int col = 0; col < playerView.GetLength(1); col++)
            {
                Console.Write($"{col + 1} ");
            }
            Console.WriteLine();

            for (int row = 0; row < playerView.GetLength(0); row++)
            {
                Console.Write($"{row + 1} ");
                for (int col = 0; col < playerView.GetLength(1); col++)
                {
                    switch (playerView[row, col])
                    {
                        case tile.empty:
                            Console.Write("~ ");
                            break;
                        case tile.hit:
                            Console.Write("X ");
                            break;
                        case tile.miss:
                            Console.Write("O ");
                            break;
                        default:
                            Console.Write("~ "); // Default to empty for unknown tiles
                            break;
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine("press enter to continue");
            Console.ReadLine();
           // Console.Clear();
        }

    }
    class Create
{
    public static void InitializeGrid(tile[,] grid)
    {
        for (int row = 0; row < grid.GetLength(0); row++)
        {
            for (int col = 0; col < grid.GetLength(1); col++)

            {
                grid[row, col] = tile.empty; 
            }
        }
    }
    public static void PlaceBoats(string playerName, tile[,] grid)
    {
        Console.Clear();
        Display.DisplayGrid($"{playerName} Grid", grid);


        for (int i = 0; i < 5; i++)
        {
                Display.DisplayGrid(playerName,grid);
            Console.WriteLine($"\n{playerName}, Place Boat {i + 1}");
            int row, col;
            bool validInput = false;

            do
            {
                Console.Write("Enter row (1-8): ");
                string rowInput = Console.ReadLine();

                Console.Write("Enter column (1-8): ");
                string colInput = Console.ReadLine();

                if (int.TryParse(rowInput, out row) && int.TryParse(colInput, out col))
                {
                    row--;
                    col--;


                    if (row >= 0 && row < grid.GetLength(0) && col >= 0 && col < grid.GetLength(1))
                    {
                        validInput = true;


                        if (grid[row, col] == tile.empty)
                        {
                            grid[row, col] = tile.boat;
                            Console.WriteLine("Boat placed successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid placement. Cell already occupied. Try again.");
                            validInput = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Row and column must be between 1 and 8. Try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter valid integers for row and column. Try again.");
                }
            } while (validInput != true);
        }
    }

    public static void PlaceRandomBoats( tile[,] computergrid)
    {
        Random rand = new Random();

        for (int i = 0; i < 5; i++)
        {
            int row, col;

            do
            {
                row = rand.Next(0, 8);
                col = rand.Next(0, 8);
            } while (computergrid[row, col] != tile.empty);

            computergrid[row, col] = tile.empty;
            // Use the hit or miss grid passed as a parameter
            computergrid[row, col] = tile.boat;
        }
    }
}
class Player
{
    public static void PlayerTurn(string playerName,tile[,] playerGrid,bool turn,tile[,] computerGrid)
    {
        int rowShot = 0, colShot = 0; // Initialize colShot with a default value
            
            
        do
        {
                Console.WriteLine("You can either: \n - shoot\n - save and exit game\n - exit game");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "save and exit game":
                        Save.ToFile(playerName,playerGrid,computerGrid);
                        Menu.ExitGame();
                        break;
                    case "exit game":
                        Menu.ExitGame();
                        break;
                    case "shoot":
                        shoot(playerName,rowShot, colShot, computerGrid, playerGrid, turn);
                        break;
                    default:
                        break;
                }
                

            

           

        } while (colShot >= 0 && rowShot >= 0 && playerGrid[rowShot, colShot] == tile.hit);
    }
        public static void shoot(string playerName, int rowShot, int colShot, tile[,] computerGrid, tile[,] playerGrid, bool turn)
        {
            Console.WriteLine($"{playerName}, it's your turn. Enter the coordinates to shoot at.");
            var tempgrid = computerGrid;
            Display.PlayerViewGrid(playerGrid, tempgrid); // Provide both playerGrid and tempgrid as parameters

            bool validInput = true;

            do
            {
                Console.Write("Enter row (1-8): ");
                string rowInput = Console.ReadLine();

                Console.Write("Enter column (1-8): ");
                string colInput = Console.ReadLine();

                if (int.TryParse(rowInput, out rowShot) && int.TryParse(colInput, out colShot))
                {
                    rowShot--;
                    colShot--;

                    if (rowShot >= 0 && rowShot < computerGrid.GetLength(0) && colShot >= 0 && colShot < computerGrid.GetLength(1))
                    {
                        if (computerGrid[rowShot, colShot] != tile.hit && computerGrid[rowShot, colShot] != tile.miss)
                        {
                            if (computerGrid[rowShot, colShot] == tile.boat)
                            {
                                Console.WriteLine("Hit!");
                                computerGrid[rowShot, colShot] = tile.hit;
                                Game.checkWin(turn, playerGrid, computerGrid);
                            }
                            else
                            {
                                Console.WriteLine("Miss!");
                                computerGrid[rowShot, colShot] = tile.miss;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid shot. You've already shot at this location. Try again.");
                            validInput = false; // Set validInput to false to continue the loop
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Row and column must be between 1 and 8. Try again.");
                        validInput = false; // Set validInput to false to continue the loop
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter valid integers for row and column. Try again.");
                    validInput = false; // Set validInput to false to continue the loop
                }
            } while (!validInput);
        }


    }
    class Computer
{
        public static void computerTurn(tile[,] playerGrid, tile[,] computerGrid, bool turn)
        {
            int rowShot, colShot = 0; // Initialize colShot with a default value

            do
            {
                Random rand = new Random();
                rowShot = rand.Next(0, 8);
                colShot = rand.Next(0, 8);

                if (playerGrid[rowShot, colShot] == tile.empty)
                {
                    if (playerGrid[rowShot, colShot] == tile.boat)
                    {
                        Console.Clear();
                        
                        Console.WriteLine("Computer hit!");
                        playerGrid[rowShot, colShot] = tile.hit;
                        Game.checkWin(turn,playerGrid, computerGrid);
                    }
                    else
                    {
                        Console.Clear();
                        
                        Console.WriteLine("Computer missed!");
                        playerGrid[rowShot, colShot] = tile.miss;
                    }
                }
                else
                {
                    // Handle the case where the computer already shot at this location
                    Console.Clear();
                    
                    Console.WriteLine("Invalid shot. The computer already shot at this location. Trying again.");
                }
                Console.ReadLine(); // Add a delay for better visibility
            } while (computerGrid[rowShot, colShot] == tile.hit); // Continue the loop only if it's a hit
        }
    }
}

