using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using NEAT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEAT
{
    public class Generation : IGeneration
    {
        public IPopulation Population { get; set; }
        public IGenome FittestGenome { get; set; }

        public Generation(IPopulation population)
        {
            Population = population;

            // Perform initial speciation
            Population.Speciate();
        }

        public IGeneration Evolve(Func<IGenome, double> getFitness)
        {
            IGeneration nextGeneration = new Generation(new Population());
            
            Population.Evaluate(getFitness);

            int spareOffspring = 0; 
            Population.Species.ForEach(s =>
            {
                s.Age++;

                if (s.LastImprovementAge > SpeciesParameters.DropOffAge)
                {
                    spareOffspring += s.ExpectedOffspring;
                    s.ExpectedOffspring = 0;
                }
            });

            if (spareOffspring > 0)
                Population.BestSpecies.ExpectedOffspring += spareOffspring;

            // Compensate for floating point inaccuracy
            while (Population.ExpectedOffspring < Population.Genomes.Count)
                Population.BestSpecies.ExpectedOffspring++;

            // Create the Offspring for each species
            List<ISpecies> populationSpecies = Population.Species.ToList();

            populationSpecies.ForEach(s =>
            {
                bool championSelected = false;

                IEnumerable<IGenome> speciesGenomes = s.NaturalSelect();

                // Create the offspring
                Enumerable.Range(0, s.ExpectedOffspring).ToList().ForEach(i =>
                {
                    IGenome offspring = null;

                    if (s.ExpectedOffspring > GenerationParameters.ChampionCopyMinimumGenomes && !championSelected)
                    {
                        offspring = speciesGenomes.OrderByDescending(p => p.Fitness / s.Genomes.Count).First();
                        championSelected = true;
                    }
                    else if (new Random().NextDouble() < GenerationParameters.MutateOnlyProbability || s.Genomes.Count == 1)
                    {
                        // Mutate
                        IGenome candidate = GetRandomGenomeBiasFitness(speciesGenomes);
                        
                        Mutate(candidate, GetSoftenFactor(candidate.Fitness, speciesGenomes));

                        offspring = candidate;
                    }
                    else
                    {
                        IGenome parent = null;
                        IGenome mate = null;

                        // Mate
                        if (new Random().NextDouble() < GenerationParameters.InterspeciesBreedingRate)
                        {
                            // Interspecies mating
                            parent = GetRandomGenomeBiasFitness(speciesGenomes);
                            mate = GetRandomGenomeBiasFitness(speciesGenomes);

                            offspring = parent.Breed(mate);
                        }
                        else
                        {
                            // Mate outside of species
                            ISpecies speciesMate = GetRandomSpeciesBiasFitness(Population.Species.Where(sp => sp != s));

                            parent = GetRandomGenomeBiasFitness(speciesGenomes);
                            mate = GetRandomGenomeBiasFitness(speciesMate.NaturalSelect());

                            offspring = parent.Breed(mate);
                        }

                        if (new Random().NextDouble() < GenerationParameters.MutateOffspringProbability || parent == mate)
                            Mutate(offspring, GetSoftenFactor(Math.Max(parent.Fitness, mate.Fitness), speciesGenomes));
                    }

                    if (offspring != null)
                    {
                        IGenome genome = offspring.Copy();

                        // Speciate the offspring against the current population
                        // This way we can see what species need to be carried over into the next generation population
                        Population.Speciate(genome);
                        ISpecies species = Population.Species.First(sp => sp.Id == genome.SpeciesId).CopyWithoutGenomes();

                        if (!nextGeneration.Population.Species.Contains(species, new SpeciesComparer()))
                            nextGeneration.Population.Species.Add(species);

                        nextGeneration.Population.Species.First(sp => sp.Id == species.Id).Genomes.Add(genome);
                        nextGeneration.Population.Genomes.Add(genome);
                    }
                });
            });
            
            nextGeneration.FittestGenome = Population.Genomes.OrderByDescending(p => p.Fitness).First();

            return nextGeneration;
        }

        private double GetSoftenFactor(double fitness, IEnumerable<IGenome> genomes)
        {
            double fitnessThreshold = RandomHelpers.GetRandomBiasingLow(1, 5);
            fitnessThreshold = genomes.Max(g => g.Fitness) / fitnessThreshold;

            return fitness > fitnessThreshold ? new Random().Next(2, 4) : 1;
        }

        private IGenome GetRandomGenomeBiasFitness(IEnumerable<IGenome> genomes)
        {
            double fitnessThreshold = RandomHelpers.GetRandomBiasingLow(1, 5);
            fitnessThreshold = genomes.Max(g => g.Fitness) / fitnessThreshold;

            return genomes.OrderBy(g => g.Fitness).Where(g => g.Fitness >= fitnessThreshold).First();
        }

        private ISpecies GetRandomSpeciesBiasFitness(IEnumerable<ISpecies> species)
        {
            if (!species.Any())
                return Population.BestSpecies;

            double fitnessThreshold = RandomHelpers.GetRandomBiasingLow(1, 5);
            fitnessThreshold = species.Max(s => s.Fitness) / fitnessThreshold;

            return species.OrderBy(s => s.Fitness).Where(s => s.Fitness >= fitnessThreshold).First();
        }

        private void Mutate(IGenome genome, double soften = 1)
        {
            if (new Random().NextDouble() < GenerationParameters.MutateAddNodeProbability / Math.Max(soften, 1))
                genome.MutateAddNode();
            else if (new Random().NextDouble() < GenerationParameters.MutateAddGeneProbability / Math.Max(soften, 1))
                genome.MutateAddGene(Population.Genomes.SelectMany(g => g.Genes));
            else
            {
                if (new Random().NextDouble() < GenerationParameters.MutateAllGeneWeightsProbability)
                    genome.MutateGeneWeights(GeneMutationType.AdjustWeight, GeneParameters.MutateGeneWeightFactor, 1);

                if (new Random().NextDouble() < GenerationParameters.MutateGeneExpressionProbability)
                    genome.MutateGeneExpression();

                if (new Random().NextDouble() < GenerationParameters.MutateGeneReExpressionProbability)
                    genome.MutateGeneReExpress();

                IGene geneCandidate = genome.Genes[new Random().Next(genome.Genes.Count)];

                if (new Random().NextDouble() < GenerationParameters.MutateGeneWeightProbability)
                    geneCandidate.MutateWeight(GeneMutationType.AdjustWeight, GeneParameters.MutateGeneWeightFactor);
            }
        }
    }
}
