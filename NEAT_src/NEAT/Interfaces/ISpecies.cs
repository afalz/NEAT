using System;
using System.Collections.Generic;

namespace NEAT.Interfaces
{
    public interface ISpecies
    {
        Guid Id { get; set; }
        List<IGenome> Genomes { get; set; }
        double MaxFitness { get; set; }
        double Fitness { get; set; }
        int ExpectedOffspring { get; set; }
        int Age { get; set; }
        int LastImprovementAge { get; set; }
        
        void SetFitness();
        void SetExpectedOffspring();
        List<IGenome> NaturalSelect();
        ISpecies CopyWithoutGenomes();
    }
}
