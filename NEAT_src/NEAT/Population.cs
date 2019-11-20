using NEAT.Genotype;
using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEAT
{
    public class Population : IPopulation
    {
        public List<IGenome> Genomes { get; set; }
        public List<ISpecies> Species { get; set; }

        public double Fitness { get; set; }
        public double AverageFitness { get; set; }
        public double ExpectedOffspring { get { return Species.Sum(s => s.ExpectedOffspring); } }

        public ISpecies BestSpecies {  get { return Species.OrderByDescending(s => s.Fitness).First(); } }

        private int numberOfInputs;
        private int numberOfOutputs;

        public Population()
        {
            Genomes = new List<IGenome>();
            Species = new List<ISpecies>();
        }

        public Population(int inputs, int outputs) : this()
        {
            numberOfInputs = inputs;
            numberOfOutputs = outputs;
        }

        public void Spawn(int size, IGenome initialGenome = null)
        {
            IGenome genome = initialGenome ?? new Genome(numberOfInputs, numberOfOutputs);

            if (initialGenome == null)
                genome.MutateGeneWeights(GeneMutationType.ReplaceWeight, 1, 1);

            Genomes = Enumerable.Range(0, size).Select(i => genome.Copy()).ToList();
        }

        public void Speciate(IGenome genome)
        {
            bool newSpecies = true;

            // Remove from current species
            if (genome.SpeciesId.HasValue)
                Species.First(s => s.Id == genome.SpeciesId).Genomes.Remove(genome);

            foreach (ISpecies species in Species.Where(s => s.Genomes.Any()))
            {
                IGenome representative = species.Genomes.First();

                if (genome.IsCompatible(representative))
                {
                    species.Genomes.Add(genome);
                    genome.SpeciesId = species.Id;

                    newSpecies = false;

                    break;
                }
            };

            if (newSpecies)
            {
                ISpecies species = new Species();

                species.Genomes.Add(genome);
                genome.SpeciesId = species.Id;

                Species.Add(species);
            }
        }

        public void Speciate()
        {
            Genomes.ToList().ForEach(genome => Speciate(genome));

            // Clean up any Species with no Genomes
            Species = Species.Where(s => s.Genomes.Any()).ToList();
        }

        public void Evaluate(Func<IGenome, double> getFitness)
        {
            // Genome Fitness
            Genomes.ForEach(g => g.Fitness = getFitness(g));

            // Species Fitness
            Species.ForEach(s => s.SetFitness());

            // Population Fitness
            Fitness = Genomes.Sum(g => g.Fitness);

            AverageFitness = Genomes.Average(g => g.Fitness);
            AverageFitness = AverageFitness == 0 ? 1 : AverageFitness;

            // Genome Expected Offspring
            Genomes.ForEach(g => g.ExpectedOffspring = g.Fitness / AverageFitness);

            // Species Expected Offspring
            Species.ForEach(s => s.SetExpectedOffspring());
        }
    }
}
