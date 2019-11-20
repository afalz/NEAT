using NEAT.Interfaces;
using System;
using System.Linq;

namespace NEAT.Runner.Simulations
{
    public class XorSimulation : ISimulation
    {
        public IGenome Simulate()
        {
            // Spawn the population
            IPopulation population = new Population(2, 1);
            population.Spawn(150);

            IGeneration generation = new Generation(population);

            int[,] xor = new int[2, 2];
            xor[0, 0] = 0;
            xor[0, 1] = 1;
            xor[1, 0] = 1;
            xor[1, 1] = 0;

            bool success = false;
            IGenome fittestGenome = null;

            Enumerable.Range(1, 200).ToList().ForEach(i =>
            {
                if (!success)
                {
                    if (generation.FittestGenome != null &&
                            generation.FittestGenome.Activate(new double[] { 0, 0 })[0] < 0.5 &&
                            generation.FittestGenome.Activate(new double[] { 0, 1 })[0] >= 0.5 &&
                            generation.FittestGenome.Activate(new double[] { 1, 0 })[0] >= 0.5 &&
                            generation.FittestGenome.Activate(new double[] { 1, 1 })[0] < 0.5)
                    {
                        success = true;
                        fittestGenome = generation.FittestGenome;
                    }
                    else
                    {
                        generation = generation.Evolve(g =>
                        {
                            double error = 0;

                            Enumerable.Range(0, 2).ToList().ForEach(x =>
                            {
                                Enumerable.Range(0, 2).ToList().ForEach(y =>
                                {
                                    int r = xor[x, y];
                                    double[] o = g.Activate(new double[] { x, y });

                                    double distanceFromAnswer = Math.Abs(r - Math.Min(1, Math.Round(o[0], 4)));

                                    error += distanceFromAnswer;
                                });
                            });

                            return Math.Pow(4 - error, 2);
                        });
                    }
                }
            });

            return fittestGenome;
        }
    }
}
