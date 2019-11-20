using NEAT.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEAT
{
    public class Species : ISpecies
    {
        public Guid Id { get; set; }
        public List<IGenome> Genomes { get; set; }
        public double MaxFitness { get; set; }
        public double Fitness { get; set; }
        public int ExpectedOffspring { get; set; }
        public int Age { get; set; }
        public int LastImprovementAge { get; set; }

        public Species()
        {
            Id = Guid.NewGuid();
            Genomes = new List<IGenome>();
        }

        public void SetFitness()
        {
            int ageDebt = (Age - LastImprovementAge + 1) - SpeciesParameters.DropOffAge;
            ageDebt = ageDebt == 0 ? 1 : ageDebt;

            Genomes.ForEach(g =>
            {
                if (ageDebt >= 1)
                    g.Fitness *= SpeciesParameters.DropOffAgePenaltyFactor;

                if (Age <= 10)
                    g.Fitness *= SpeciesParameters.AgeSignificance;

                if (g.Fitness < 0)
                    g.Fitness = 0.0001;

                g.Fitness /= Genomes.Count;
            });

            Fitness = Genomes.Any() ? Genomes.Average(g => g.Fitness) : 0;

            if (Fitness > MaxFitness)
            {
                MaxFitness = Fitness;
                LastImprovementAge = 0;
            }
            else
                LastImprovementAge++;
        }
        
        public void SetExpectedOffspring()
        {
            double skim = 0;
            
            ExpectedOffspring = Genomes.Sum(g => GetSkimmedExpectedOffspring(ref skim, g.ExpectedOffspring));
        }

        private int GetSkimmedExpectedOffspring(ref double skim, double offspring)
        {
            int wholeOffspring = (int)Math.Floor(offspring);
            double fractionalOffspring = offspring % 1.0;

            skim += fractionalOffspring;

            if (skim > 1)
            {
                int wholeSkim = (int)Math.Floor(skim);
                wholeOffspring += wholeSkim;
                skim -= wholeSkim;
            }

            return wholeOffspring;
        }

        public List<IGenome> NaturalSelect()
        {
            int strongest = (int)Math.Floor((SpeciesParameters.NaturalSelectionFactor * (Genomes.Count)) + 1);

            return Genomes.OrderByDescending(g => g.Fitness).Take(strongest).ToList();
        }

        public ISpecies CopyWithoutGenomes()
        {
            return new Species()
            {
                Id = Id,
                Age = Age,
                Fitness = Fitness,
                MaxFitness = MaxFitness,
                LastImprovementAge = LastImprovementAge
            };
        }
    }

    public class SpeciesComparer : IEqualityComparer<ISpecies>
    {
        public bool Equals(ISpecies a, ISpecies b)
        {
            return a.Id == b.Id;
        }

        public int GetHashCode(ISpecies a)
        {
            return a.Id.GetHashCode();
        }
    }
}
