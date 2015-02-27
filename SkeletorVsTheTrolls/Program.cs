﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SkeletorCsTheTrolls
{
    class Program
    {
        static void Main(string[] args)
        {
            bool notQuit = true;
            do
            {
                Game game = new Game();
                game.PlayGame();
                Console.Write("Do you want to play again? (y/n)");
                string result = Console.ReadLine();
                result = result.ToLower();
                if (result == "n" || result == "no") notQuit = false;
                if (result.Contains("no")) notQuit = false;
            } while (notQuit);
        }
    }

    /// <summary>
    /// Determines the what's in the point
    /// </summary>
    enum PointStatus
    {
        Empty, Cheese, Mouse, Cat, CatAndCheese
    }

    /// <summary>
    /// This represents a single cell in the grid.
    /// </summary>
    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public PointStatus Status { get; set; }

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Status = PointStatus.Empty;
        }
    }

    /// <summary>
    /// The hero of our game and the responible is holding it's status
    /// </summary>
    class Mouse
    {
        public Point Position { get; set; }  // the current position of the hero
        public int Energy { get; set; }      // How much the hero has
        public bool HasBeenPouncedOn { get; set; } // whether or not if the enemy has got our heros

        public Mouse()
        {
            Energy = 100;    // setting the energy to 50
        }
    }

    /// <summary>
    /// Our trolling enemy
    /// </summary>
    class Cat
    {
        public Point Position { get; set; }     // the current of this enemy

        public Cat() { }   // nothing is created in the constructor for this kitty
    }

    /// <summary>
    /// The meat of game and where The Grid owns the mouse, the cheese, and the cats.
    /// </summary>
    class Level
    {
        // const variables
        protected int WIDTH = 50;
        protected int HEIGHT = 25;
        // properties
        public Point[,] TheGrid;  // a 2D array 
        public Mouse TheHero;     // our hero
        public Point TheGoal;      // the goal for our hero
        public int CheeseCount;   // the counter for the levels or cheese counter
        public List<Cat> Enemies;    // a list of our enemies or cats
        public int levelNumber = 0;  // a counter for the level and which level is created
        public Random RNG = new Random();

        // the constructor
        public Level()
        {
            // initialize and set each index with a point
            TheGrid = new Point[WIDTH, HEIGHT];

            if (levelNumber == 0)
            {
                // replace this with a call to the level loader
                for (int y = 0; y < HEIGHT; y++)
                {
                    for (int x = 0; x < WIDTH; x++)
                    {
                        TheGrid[x, y] = new Point(x, y);
                    }
                }
            }

            Console.WriteLine();
                TheHero = new Mouse();  // initialize the hero
            TheHero.Position = TheGrid[RNG.Next(0, TheGrid.GetLength(0)), RNG.Next(0, TheGrid.GetLength(1))];  // randomly throw our hero at a position
            TheHero.Position.Status = PointStatus.Mouse;

            // place the goal or the cheese
            PlaceCheese();

            Enemies = new List<Cat>();  // A placeholder for our cats or enemies
        }

        public void LevelLoader()
        {
            StreamReader reader = new StreamReader("levels.txt");

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (line.Contains("level"))
                {
                }
            }
        }

        /// <summary>
        /// Draws our playing field
        /// </summary>
        public void DrawGrid()
        {
            // clear the console
            Console.Clear();

            // display the gride to the screen
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int x = 0; x < WIDTH; x++)
                {
                    switch (this.TheGrid[x, y].Status)
                    {
                        case PointStatus.Mouse:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write("@");  // printing the hero
                            break;
                        case PointStatus.Cheese:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("O");  // printing the goal
                            break;
                        case PointStatus.Cat:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("☠");  // print the kitty or the enemy
                            break;
                        case PointStatus.CatAndCheese:
                            Console.Write("n");  // print the cat and the cheese
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write(" ");  // print the walkable spaces
                            //Console.Write(Char.ConvertFromUtf32(176));
                            break;
                    }
                }
                Console.WriteLine();
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            for (int x = 0; x < WIDTH; x++)
            {
                Console.Write("=");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Gets the input for the user
        /// </summary>
        /// <returns></returns>
        public ConsoleKey GetUserMove()
        {
            ConsoleKeyInfo input = Console.ReadKey(true);
            while (!ValidMove(input.Key))
            {
                Console.WriteLine("Invalid move");
                System.Threading.Thread.Sleep(750);
                input = Console.ReadKey(true);
            }

            return input.Key;
        }

        public bool ValidMove(ConsoleKey input)
        {
            switch (input)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    if (this.TheHero.Position.Y - 1 >= 0)
                    {
                        return true;
                    }
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    if (this.TheHero.Position.Y + 1 < TheGrid.GetLength(1))
                    {
                        return true;
                    }
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    if (this.TheHero.Position.X - 1 >= 0)
                    {
                        return true;
                    }
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    if (this.TheHero.Position.X + 1 < TheGrid.GetLength(0))
                    {
                        return true;
                    }
                    break;
                default:
                    Console.WriteLine("Exceed Boundary");
                    System.Threading.Thread.Sleep(750);
                    break;
            }
            return false;
        }

        public void MoveMouse(ConsoleKey input)
        {
            int newMouseX = this.TheHero.Position.X;
            int newMouseY = this.TheHero.Position.Y;

            // makes a temportary move
            switch (input)
            {
                case ConsoleKey.W:
                case ConsoleKey.UpArrow:
                    newMouseY--;
                    break;
                case ConsoleKey.S:
                case ConsoleKey.DownArrow:
                    newMouseY++;
                    break;
                case ConsoleKey.A:
                case ConsoleKey.LeftArrow:
                    newMouseX--;
                    break;
                case ConsoleKey.D:
                case ConsoleKey.RightArrow:
                    newMouseX++;
                    break;
                default:
                    Console.WriteLine("Exceed Boundary");
                    break;
            }
            // checks for the cheese
            if (this.TheGrid[newMouseX, newMouseY].Status == PointStatus.Cheese)
            {
                CheeseCount++;
                this.TheHero.Energy += 5;
                this.TheGrid[newMouseX, newMouseY].Status = PointStatus.Empty;
                PlaceCheese();  // find a new position for the cheese
                if (CheeseCount % 2 == 0)
                {
                    AddCat();
                }
            }
            // set the old mouse position to empty
            this.TheHero.Position.Status = PointStatus.Empty;
            // set the new mouse position to the mouse
            this.TheGrid[newMouseX, newMouseY].Status = PointStatus.Mouse;
            // update the values of the mouse
            this.TheHero.Position = this.TheGrid[newMouseX, newMouseY];
            this.TheHero.Energy--;  // decrease the mouse's energy
        }

        /// <summary>
        /// Places the cheese at an empty position on the grid
        /// </summary>
        public void PlaceCheese()
        {
            do
            {
                this.TheGoal = TheGrid[RNG.Next(0, TheGrid.GetLength(0)), RNG.Next(0, TheGrid.GetLength(1))];  // a point to check
            } while (this.TheGoal.Status != PointStatus.Empty);  // when the position is met
            this.TheGoal.Status = PointStatus.Cheese; // set the reference to the cheese
        }

        /// <summary>
        /// Places a cat on a heap and on the grid on a empty position.
        /// </summary>
        public void AddCat()
        {
            Cat BadKats = new Cat();
            Enemies.Add(BadKats);
            PlaceCat();

        }

        /// <summary>
        /// Places a cat on the grid at a random position on the grid
        /// </summary>
        public void PlaceCat()
        {
            do
            {
                this.Enemies.Last().Position = TheGrid[RNG.Next(0, TheGrid.GetLength(0)), RNG.Next(0, TheGrid.GetLength(1))];
            } while (this.Enemies.Last().Position.Status != PointStatus.Empty);
            this.Enemies.Last().Position.Status = PointStatus.Cat;
        }

        public void MoveCat(Cat cat)
        {
            int chance = RNG.Next(1, 100);
            int diffX = this.TheHero.Position.X - cat.Position.X;
            int diffY = this.TheHero.Position.Y - cat.Position.Y;
            // new placeholder position if the information of the grid
            Point newPos = TheGrid[cat.Position.X, cat.Position.Y];

            // 80% chance a cat will move
            if (chance <= 80)  // it's immpossible for the player to play
            //if (chance <= 50)
            {
                if (diffX != 0)  // relative position two each object of the position X
                {
                    if (diffX < 0 && cat.Position.X - 1 >= 0)  // the mouse can move to the left
                    {
                        newPos.X--;
                    }
                    if (diffX > 0 && cat.Position.X + 1 < TheGrid.GetLength(1))  // the mouse can move to the right
                    {
                        newPos.X++;
                    }
                }
                if (diffY != 0)  // relative position two each object of the position Y
                {
                    if (diffY < 0 && cat.Position.Y - 1 >= 0)  // the mouse can move up
                    {
                        newPos.Y--;
                    }
                    if (diffY > 0 && cat.Position.Y + 1 < TheGrid.GetLength(0))  // the mouse can move down
                    {
                        newPos.Y++;
                    }
                }
            }
            // this check if the status is CatAndCheese and the cat is leaving the cheese.
            if (newPos.Status == PointStatus.CatAndCheese)
            {
                newPos.Status = PointStatus.Cheese;
            }
            // getting the new position on the grid
            newPos = TheGrid[newPos.X, newPos.Y];
            // check the cat and the cheese are at the same position
            if (newPos.Status == PointStatus.Cheese)
            {
                TheGrid[newPos.X, newPos.Y].Status = PointStatus.CatAndCheese;
            }
            // check the cat on the mouse
            if (newPos.Status == PointStatus.Mouse)
            {
                this.TheHero.HasBeenPouncedOn = true;
            }
            // update the cat's position via the grid
            cat.Position.Status = PointStatus.Empty;    // update the grid with empty
            // set the new mouse position to the mouse
            //this.TheGrid[newPos.X, newPos.Y].Status = PointStatus.Cat;
            newPos.Status = PointStatus.Cat;
            // update the values of the mouse
            cat.Position = newPos;
        }

        public void PlayGame()
        {
            while (!this.TheHero.HasBeenPouncedOn && this.TheHero.Energy > 0)
            {
                DrawGrid();
                Console.WriteLine("Energy: {0}", this.TheHero.Energy);
                MoveMouse(GetUserMove());
                foreach (Cat Kats in Enemies)
                {
                    MoveCat(Kats);
                }
            }
            if (this.TheHero.HasBeenPouncedOn)
            {
                Console.WriteLine("You are in the cat's stomach.");
            }
            if (this.TheHero.Energy == 0)
            {
                Console.WriteLine("You are too weak to continue.");
            }
        }
    }

    enum GameState
    {
        Field, Battle
    }
    class Game
    {
        // const variables
        protected int WIDTH = 90;
        protected int HEIGHT = 35;
        // properties
        Level level = new Level();
        GameState state { get; set; }

        public Game()
        {
            Console.SetWindowSize(WIDTH, HEIGHT);
            state = GameState.Field;
            // call levelLoader()

        }
        public void PlayGame()
        {
            int speed = 0;
            while (!level.TheHero.HasBeenPouncedOn && level.TheHero.Energy > 0)
            {
                level.DrawGrid();
                Console.WriteLine("Energy: {0}", level.TheHero.Energy);
                level.MoveMouse(level.GetUserMove());
                foreach (Cat Kats in level.Enemies)
                {
                    level.MoveCat(Kats);
                }
            }
            if (level.TheHero.HasBeenPouncedOn)
            {
                Console.WriteLine("You are in the cat's stomach.");
            }
            if (level.TheHero.Energy == 0)
            {
                Console.WriteLine("You are too weak to continue.");
            }
        }
    }
}
