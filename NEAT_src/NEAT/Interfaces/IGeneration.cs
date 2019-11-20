using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Interfaces
{
    public interface IGeneration
    {
        IPopulation Population { get; set; }
        IGenome FittestGenome { get; set; }

        IGeneration Evolve(Func<IGenome, double> getFitness);
    }
}
