using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEAT.Interfaces;
using NEAT.Utilities;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;

namespace NEAT.Test
{
    [TestClass]
    public class GenerationTest : Test
    {
        private IPopulation population;

        [TestInitialize]
        public new void Init()
        {
            population = new Population(2, 1);
        }

        [TestMethod]
        public void EvolveTestXOR()
        {
            // Spawn the population
            population.Spawn(150);

            IGeneration generation = new Generation(population);

            int[,] xor = new int[2, 2];
            xor[0, 0] = 0;
            xor[0, 1] = 1;
            xor[1, 0] = 1;
            xor[1, 1] = 0;

            bool success = false;

            //Debug.WriteLine(DotGraph.Get(population.Genomes.First()));

            Enumerable.Range(1, 100).ToList().ForEach(i =>
            {
                //if (generation.FittestGenome != null)
                //    Debug.WriteLine(DotGraph.Get(generation.FittestGenome));

                if (!success)
                {
                    //Debug.WriteLine("(G" + i + ")" + "Genomes: " + generation.Population.Genomes.Count);

                    if (generation.FittestGenome != null &&
                            generation.FittestGenome.Activate(new double[] { 0, 0 })[0] < 0.5 &&
                            generation.FittestGenome.Activate(new double[] { 0, 1 })[0] >= 0.5 &&
                            generation.FittestGenome.Activate(new double[] { 1, 0 })[0] >= 0.5 &&
                            generation.FittestGenome.Activate(new double[] { 1, 1 })[0] < 0.5)
                    {
                        success = true;

                        string json = JsonConvert.SerializeObject(generation.FittestGenome);

                        Debug.WriteLine("Success!");
                        Debug.WriteLine(json);
                        Debug.WriteLine(DotGraph.Get(generation.FittestGenome));
                        Debug.WriteLine(i);
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
        }

        [TestMethod]
        public void EvolveTestBitPosition()
        {
            GenomeParameters.AverageWeightDifferenceFactor = 3;
            GenomeParameters.CompatibilityDistanceThreshold = 4;

            // Spawn the population
            population = new Population(4, 4);
            population.Spawn(300);

            IGeneration generation = new Generation(population);

            bool success = false;

            Enumerable.Range(1, 500).ToList().ForEach(i =>
            {
                //Debug.WriteLine("(G" + i + ")" + "Genomes: " + generation.Population.Genomes.Count);

                if (!success)
                {
                    double[] output1 = new double[]{};
                    double[] output2 = new double[]{};
                    double[] output3 = new double[]{};
                    double[] output4 = new double[]{};

                    if (generation.FittestGenome != null)
                    {
                        output1 = generation.FittestGenome.Activate(new double[] { 1, 0, 0, 0 });
                        output2 = generation.FittestGenome.Activate(new double[] { 0, 1, 0, 0 });
                        output3 = generation.FittestGenome.Activate(new double[] { 0, 0, 1, 0 });
                        output4 = generation.FittestGenome.Activate(new double[] { 0, 0, 0, 1 });
                    }
                    
                    if (generation.FittestGenome != null &&
                            output1[0] >= 0.5 &&
                            output1[1] < 0.5 &&
                            output1[2] < 0.5 &&
                            output1[3] < 0.5 &&
                            output2[0] < 0.5 &&
                            output2[1] >= 0.5 &&
                            output2[2] < 0.5 &&
                            output2[3] < 0.5 &&
                            output3[0] < 0.5 &&
                            output3[1] < 0.5 &&
                            output3[2] >= 0.5 &&
                            output3[3] < 0.5 &&
                            output4[0] < 0.5 &&
                            output4[1] < 0.5 &&
                            output4[2] < 0.5 &&
                            output4[3] >= 0.5)
                    {
                        success = true;

                        string json = JsonConvert.SerializeObject(generation.FittestGenome);

                        Debug.WriteLine("Success!");
                        Debug.WriteLine(json);
                        Debug.WriteLine(DotGraph.Get(generation.FittestGenome));
                        Debug.WriteLine(i);
                    }
                    else
                    {
                        generation = generation.Evolve(g =>
                        {
                            double error = 0;

                            Enumerable.Range(0, 4).ToList().ForEach(x =>
                            {
                                double[] bitPos = new double[] { 0, 0, 0, 0 };
                                bitPos[x]++;

                                double[] o = g.Activate(bitPos);
                                
                                double distanceFromAnswer = 0;

                                for (int y = 0; y <= 3; y++)
                                {
                                    double r = double.IsNaN(o[y]) ? 0 : Math.Min(1, Math.Round(o[y], 4));

                                    if (y == x)
                                        distanceFromAnswer += Math.Abs(1 - r);
                                    else
                                        distanceFromAnswer += Math.Abs(0 - r);
                                }

                                error += distanceFromAnswer;
                            });

                            return Math.Pow(Math.Max(4 - error, 0), 2);
                        });
                    }
                }
            });
        }
    }
}
