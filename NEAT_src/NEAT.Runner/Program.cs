using NEAT.Interfaces;
using NEAT.Runner.Simulations;
using NEAT.Utilities;
using System;
using System.Linq;

namespace NEAT.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            ISimulation simulation = new XorSimulation();
            int simulationCount = 10;

            Console.WriteLine("Running " + simulationCount + " Simulations");

            Enumerable.Range(1, simulationCount).ToList().ForEach(i =>
            {
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Beginning Simulation: " + i);

                IGenome genome = simulation.Simulate();

                if (genome != null)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Simulation Successful!");

                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(DotGraph.Get(genome));
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Simulation Unsucessful!");
                }
            });

            Console.ReadLine();
        }
    }
}
