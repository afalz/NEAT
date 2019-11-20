using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEAT.Genotype;
using NEAT.Genotype.Constants;
using NEAT.Interfaces;

namespace NEAT.Test
{
    [TestClass]
    public class Test
    {
        protected IGenome genome;

        protected INode inputNode;
        protected INode outputNode;

        protected IGene gene;

        [TestInitialize]
        public void Init()
        {
            genome = new Genome();

            inputNode = new Node(NodeType.Input, 0);
            outputNode = new Node(NodeType.Output, 0);

            genome.Nodes.Add(inputNode);
            genome.Nodes.Add(outputNode);

            gene = new Gene(inputNode, outputNode, Genome.GetNextInnovation());

            genome.Genes.Add(gene);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Genome.ResetInnovation();
        }
    }
}
