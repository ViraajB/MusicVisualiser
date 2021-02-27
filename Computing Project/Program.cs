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
                Console.WriteLine("Enter your choice.");
                Console.WriteLine(
                    "1 : Bars Visualiser" +
                    "\n2 : Graph Visualiser" +
                    "\n3 : Exit Program"
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
