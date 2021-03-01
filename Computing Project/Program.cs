using Love;
using System;
using ConsoleTables;

namespace Computing_Project
{
    class colour //colour class so user can set the colour of the visualiser, variables are in a class so it can be used throughout the entire project
    {
        public static int r = 255; //sets default colour to red
        public static int g = 000;
        public static int b = 000;
    }
    class Program
    {
        static void Main(string[] args)
        {
            bool isDone = false;
            var table = new ConsoleTable("Option", "Outcome"); //creates the menu for me
            table.AddRow(1, "Bars Visualiser")
                .AddRow(2, "Graph Visualiser")
                .AddRow(3, "Quit");
            do
            {
                Console.Title = "Launcher";
                table.Write(Format.Alternative);
                Console.Write("Choice: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Console.WriteLine("Would you like to change the colour of the visualiser? Enter the RGB value, default colour is red with rgb values of {0} {1} {2}.", colour.r, colour.g, colour.b + "\n[y/n]");
                        string y = Console.ReadLine();
                        if (y == "y")
                        {
                            Console.WriteLine("Enter the RGB value of the colour, in the order 'red, green, blue'.");
                            colour.r = int.Parse(Console.ReadLine());
                            colour.g = int.Parse(Console.ReadLine());
                            colour.b = int.Parse(Console.ReadLine());
                        }
                        Boot.Run(new visBars()); //starts the visualiser window
                        break;

                    case "2":
                        Console.WriteLine("Would you like to change the colour of the visualiser? Enter the RGB value, default colour is red with rgb values of {0} {1} {2}.", colour.r, colour.g, colour.b + "\n[y/n]");
                        y = Console.ReadLine();
                        if (y == "y")
                        {
                            Console.WriteLine("Enter the RGB value of the colour, in the order 'red, green, blue'.");
                            colour.r = int.Parse(Console.ReadLine());
                            colour.g = int.Parse(Console.ReadLine());
                            colour.b = int.Parse(Console.ReadLine());
                        }
                        Boot.Run(new visGraph());
                        break;

                    case "3":
                        Environment.Exit(0);
                        isDone = true;
                        break;

                    default:
                        Console.WriteLine("\nInvalid input.");
                        break;
                }
            } while (isDone == false);
        }
    }
}
