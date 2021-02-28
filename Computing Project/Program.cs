using Love;
using System;

namespace Computing_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            bool isDone = false;
            do
            {
                Console.Title = "Launcher";
                Console.WriteLine("Enter your choice.");
                Console.WriteLine(
                    "\n|___|_________________|" +
                    "\n|   |                 |" +
                    "\n| 1 | Bars Visualiser |" +
                    "\n|   |                 |" +
                    "\n| 2 | Graph Visualiser|" +
                    "\n|   |                 |" +
                    "\n| 3 | Exit Program    |" +
                    "\n|   |                 |" +
                    "\n|___|_________________|" +
                    "\n"
                    );
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        Boot.Run(new visBars()); //starts the visualiser window
                        break;

                    case "2":
                        Boot.Run(new visGraph());
                        break;

                    case "3":
                        Environment.Exit(0);
                        isDone = true;
                        break;

                    default:
                        Console.WriteLine("Invalid input.");
                        break;
                }
            } while (isDone == false);
        }
    }
}
