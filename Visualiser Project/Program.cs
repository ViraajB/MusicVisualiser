using Love;
using System;
using ConsoleTables; //? remove this when you find a way to change colour without the console ?\\

namespace Visualiser_Project
{
    class colour //*colour class so user can set the colour of the visualiser, variables are in a class so it can be used throughout the entire project*\\
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
            bool b = false;

            var table = new ConsoleTable("Option", "Outcome"); //*creates the menu for me*\\
            table.AddRow(1, "Load visualiser with default settings.")
                .AddRow(2, "Change colour.")
                .AddRow(3, "Quit.");
            do
            {
                Console.Title = "Launcher";
                table.Write(Format.Alternative);
                Console.Write("Choice: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Boot.Run(new Visualiser()); //*starts the visualiser window*\\
                        break;

                    case "2":
                        Console.WriteLine("Enter the numerical RGB values of the colour you would like, current colour is {0}, {1}, {2}", colour.r, colour.g, colour.b);
                        do
                        {
                            try
                            {
                                Console.Write("R: ");
                                colour.r = int.Parse(Console.ReadLine());
                                b = true;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid input");
                            }
                        } while (b == false);
                        b = false;
                        do
                        {
                            try
                            {
                                Console.Write("G: ");
                                colour.g = int.Parse(Console.ReadLine());
                                b = true;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid input");
                            }
                        } while (b == false);
                        b = false;
                        do
                        {
                            try
                            {
                                Console.Write("B: ");
                                colour.b = int.Parse(Console.ReadLine());
                                b = true;
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Invalid input");
                            }
                        } while (b == false);
                        b = false;
                        Boot.Run(new Visualiser());
                        break;

                    case "3":
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