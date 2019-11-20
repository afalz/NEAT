using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEAT.Genotype;
using NEAT.Genotype.Constants;
using NEAT.Interfaces;

namespace NEAT.Test.Genotype
{
    [TestClass]
    public class GeneTest
    {
        [TestMethod]
        public void MutateWeightAdjustTest()
        {
            IGene gene = new Gene(new Node(NodeType.Hidden), new Node(NodeType.Hidden), 1);
            double weight = gene.Weight;

            double newWeight = gene.MutateWeight(GeneMutationType.AdjustWeight, 1);
            double adjustedWeight = weight + newWeight;

            Assert.AreEqual(gene.Weight, adjustedWeight);

            gene.MutateWeight(GeneMutationType.AdjustWeight, 0);

            Assert.AreEqual(gene.Weight, adjustedWeight);
        }

        [TestMethod]
        public void MutateWeightReplaceTest()
        {
            IGene gene = new Gene(new Node(NodeType.Hidden), new Node(NodeType.Hidden), 1);
            double weight = gene.Weight;

            double newWeight = gene.MutateWeight(GeneMutationType.ReplaceWeight, 1);

            Assert.AreEqual(gene.Weight, newWeight);

            newWeight = gene.MutateWeight(GeneMutationType.ReplaceWeight, 0);

            Assert.AreEqual(gene.Weight, 0);
        }

        [TestMethod]
        public void CopyTest()
        {
            IGene gene = new Gene(new Node(NodeType.Hidden), new Node(NodeType.Hidden), 1);
            IGene copiedGene = gene.Copy();

            Assert.AreEqual(gene.Innovation, copiedGene.Innovation);
            Assert.AreEqual(gene.IsExpressed, copiedGene.IsExpressed);
            Assert.AreEqual(gene.NodeIn.Id, copiedGene.NodeIn.Id);
            Assert.AreEqual(gene.NodeOut.Id, copiedGene.NodeOut.Id);
            Assert.AreEqual(gene.Weight, copiedGene.Weight);
        }

        [TestMethod]
        public void MutateExpressionTest()
        {
            IGene gene = new Gene(new Node(NodeType.Hidden), new Node(NodeType.Hidden), 1);
            bool isExpressed = gene.IsExpressed;

            GeneParameters.MutateExpressionProbability = 1;

            gene.MutateExpression();

            Assert.IsTrue(gene.IsExpressed == false);

            GeneParameters.MutateExpressionProbability = 0;

            gene.MutateExpression();

            Assert.IsFalse(gene.IsExpressed);
        }
    }
}
