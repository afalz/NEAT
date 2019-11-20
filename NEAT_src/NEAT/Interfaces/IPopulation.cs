using System;
using System.Collections.Generic;

namespace NEAT.Interfaces
{
    public interface IPopulation
    {
        List<IGenome> Genomes { get; set; }
        List<ISpecies> Species { get; set; }
        double Fitness { get; set; }
        double AverageFitness { get; set; }
        double ExpectedOffspring { get; }
        ISpecies BestSpecies { get; }

        void Spawn(int size, IGenome initialGenome = null);
        void Speciate();
        void Speciate(IGenome genome);
        void Evaluate(Func<IGenome, double> getFitness);
    }
}
