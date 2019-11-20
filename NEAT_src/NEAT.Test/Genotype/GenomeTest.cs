using Microsoft.VisualStudio.TestTools.UnitTesting;
using NEAT.Genotype;
using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;

namespace NEAT.Test.Genotype
{
    [TestClass]
    public class GenomeTest : Test
    {
        [TestMethod]
        public void MutateAddNodeTest()
        {
            int genomeNodeCount = genome.Nodes.Count;

            INode newNode = genome.MutateAddNode();
            
            // We should have one extra node
            Assert.AreEqual(genomeNodeCount + 1, genome.Nodes.Count);
            // Original gene should now not be expressed
            Assert.IsTrue(genome.Genes.Any(g => g.NodeIn == outputNode && g.NodeOut == inputNode && !g.IsExpressed));
            // New gene going into the new node should exist and have a weight of 1 
            Assert.IsTrue(genome.Genes.Any(g => g.Weight == 1 && g.NodeIn == newNode && g.NodeOut == inputNode));
            // New gene going out of the new node should exist and have the weight of the original gene
            Assert.IsTrue(genome.Genes.Any(g => g.Weight == gene.Weight && g.NodeIn == outputNode && g.NodeOut == newNode));
        }

        [TestMethod]
        public void MutateAddGeneTest()
        {
            // Add new input node
            INode node = new Node(NodeType.Input);
            genome.Nodes.Add(node);

            IGene newGene = null;

            while (newGene == null)
                newGene = genome.MutateAddGene(genome.Genes);

            // New gene should connect the new node and the existing output node
            Assert.IsTrue(newGene.NodeIn == node || newGene.NodeIn == outputNode);
            Assert.IsTrue(newGene.NodeOut == node || newGene.NodeOut == outputNode);
        }

        [TestMethod]
        public void BreedTest()
        {
            IGenome mate = genome.Copy();
            mate.MutateAddNode();
            mate.MutateAddGene(genome.Genes);

            genome.MutateAddNode();

            IGenome offspring = genome.Breed(mate);

            string parentGenes = string.Join("->", genome.Genes.Select(g => "A" + g.Innovation).ToList());
            string mateGenes = string.Join("->", mate.Genes.Select(g => "B" + g.Innovation).ToList());
            string offspringGenes = string.Join("->", offspring.Genes.OrderBy(g => g.Innovation).Select(g => "C" + g.Innovation).ToList());

            string dotGraph = "digraph G { " +
            "    subgraph cluster_0 {                                  " +
            "                    style = filled;                       " +
            "                    color = lightgrey;                    " +
            "                    node[style = filled, color = white];  " +
            "                    " + parentGenes + ";                  " +
            "                    label = \"Parent\";                   " +
            "                }                                         " +
            "    subgraph cluster_1 {                                  " +
            "                    style = filled;                       " +
            "                    color = lightgrey;                    " +
            "                    node[style = filled, color = white];  " +
            "                    " + mateGenes + ";                    " +
            "                    label = \"Mate\"; }                   " +
            "    subgraph cluster_2 {                                  " +
            "                    style = filled;                       " +
            "                    color = lightblue;                    " +
            "                    node[style = filled, color = white];  " +
            "                    " + offspringGenes + ";               " +
            "                    label = \"Offspring\";                " +
            " }";

            genome.Genes.ForEach(g =>
            {
                dotGraph += string.Format("\"{0}\" [ label=\"{1} {2} {3}\" shape = \"square\" ]", "A" + g.Innovation, g.Innovation, g.NodeOut.Id.ToString().Substring(32) + "->" + g.NodeIn.Id.ToString().Substring(32), Math.Round(g.Weight, 2)) + ";";
            });

            mate.Genes.ForEach(g =>
            {
                dotGraph += string.Format("\"{0}\" [ label=\"{1} {2} {3}\" shape = \"square\" ]", "B" + g.Innovation, g.Innovation, g.NodeOut.Id.ToString().Substring(32) + "->" + g.NodeIn.Id.ToString().Substring(32), Math.Round(g.Weight, 2)) + ";";
            });

            offspring.Genes.ForEach(g =>
            {
                dotGraph += string.Format("\"{0}\" [ label=\"{1} {2} {3}\" shape = \"square\" ]", "C" + g.Innovation, g.Innovation, g.NodeOut.Id.ToString().Substring(32) + "->" + g.NodeIn.Id.ToString().Substring(32), Math.Round(g.Weight, 2)) + ";";
            });

            dotGraph += " rankdir=LR; }";

            Debug.WriteLine(dotGraph);
        }
    }
}
