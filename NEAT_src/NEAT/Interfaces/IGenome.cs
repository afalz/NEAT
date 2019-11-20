using NEAT.Genotype.Constants;
using System;
using System.Collections.Generic;

namespace NEAT.Interfaces
{
    public interface IGenome
    {
        Guid Id { get; set; }
        List<INode> Nodes { get; }
        List<IGene> Genes { get; }
        double Fitness { get; set; }
        double ExpectedOffspring { get; set; }
        Guid? SpeciesId { get; set; }

        int Size();
        INode MutateAddNode();
        IGene MutateAddGene(IEnumerable<IGene> genePool);
        void MutateGeneWeights(GeneMutationType mutationType, double power, double rate);
        void MutateGeneExpression();
        void MutateGeneReExpress();
        List<IGene> GetExpressedGenesByNodeOut(INode nodeOut);
        IGenome Breed(IGenome mate);
        IGenome Copy();
        bool IsCompatible(IGenome genome);
        double[] Activate(double[] inputs, Func<double, double> activator = null);
    }
}
