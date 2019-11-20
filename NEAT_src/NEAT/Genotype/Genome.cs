using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using NEAT.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NEAT.Genotype
{
    public class Genome : IGenome
    {
        public Guid Id { get; set; }
        public List<INode> Nodes { get; set; }
        public List<IGene> Genes { get; set; }
        public double Fitness { get; set; }
        public double ExpectedOffspring { get; set; }
        public Guid? SpeciesId { get; set; }

        private static int currentInnovation = 0;

        public Genome()
        {
            Id = Guid.NewGuid();
            Nodes = new List<INode>();
            Genes = new List<IGene>();
        }

        public Genome(int inputs, int outputs, double weight = 0) : this()
        {
            List<INode> inputNodes = Enumerable.Range(0, inputs).Select<int, INode>(i => new Node(NodeType.Input, i)).ToList();
            List<INode> outputNodes = Enumerable.Range(0, outputs).Select<int, INode>(i => new Node(NodeType.Output, i)).ToList();

            Nodes = inputNodes.Concat(outputNodes).ToList();

            inputNodes.ForEach(inputNode => {
                outputNodes.ForEach(outputNode =>
                    Genes.Add(new Gene(inputNode, outputNode, GetNextInnovation(), weight)));
            });
        }

        public IGenome Copy()
        {
            Dictionary<Guid, INode> nodeReplacements = new Dictionary<Guid, INode>();
            List<IGene> genes = Genes.Select(g => g.Copy()).ToList();

            genes.ToList().ForEach(g =>
            {
                if (!nodeReplacements.Any(nr => nr.Key == g.NodeIn.Id))
                    nodeReplacements.Add(g.NodeIn.Id, g.NodeIn.Copy());

                if (!nodeReplacements.Any(nr => nr.Key == g.NodeOut.Id))
                    nodeReplacements.Add(g.NodeOut.Id, g.NodeOut.Copy());

                g.NodeIn = nodeReplacements[g.NodeIn.Id];
                g.NodeOut = nodeReplacements[g.NodeOut.Id];
            });

            return new Genome()
            {
                Id = Id,
                Genes = genes,
                Nodes = nodeReplacements.Select(nr => nr.Value).ToList()
            };
        }

        public int Size()
        {
            return Genes.ToList().Count();
        }

        public IGene MutateAddGene(IEnumerable<IGene> genePool)
        {
            // Select two nodes at random to connect
            List<INode> nodeOutCandidates = Nodes.ToList();

            if (!GenomeParameters.AllowOutputOutGenes)
                nodeOutCandidates = nodeOutCandidates.Where(n => n.Type != NodeType.Output).ToList();

            INode nodeOut = nodeOutCandidates[new Random().Next(nodeOutCandidates.Count)];

            // Get existing connections for this node
            List<IGene> existingConnectionsIn = Genes.Where(cg => cg.NodeIn == nodeOut).ToList();
            List<IGene> existingConnectionsOut = Genes.Where(cg => cg.NodeOut == nodeOut).ToList();

            // Discover nodes where no connection currently exists
            List<INode> nodeInCandidates = Nodes
                .Except(existingConnectionsIn.Select(g => g.NodeOut))
                .Except(existingConnectionsOut.Select(g => g.NodeIn)).ToList();

            // Don't allow input to input or output to output connections
            if (nodeOut.Type == NodeType.Input || nodeOut.Type == NodeType.Output)
                nodeInCandidates = nodeInCandidates.Where(n => n.Type != nodeOut.Type).ToList();

            if (nodeOut.Type == NodeType.Hidden || nodeOut.Type == NodeType.Output)
                nodeInCandidates = nodeInCandidates.Where(n => n.Type != NodeType.Input).ToList();

            if (nodeInCandidates.Count > 0)
            {
                INode selectedNodeIn = nodeInCandidates[new Random().Next(nodeInCandidates.Count)];

                IGene existingGene = genePool.FirstOrDefault(gp => gp.NodeIn.Id == selectedNodeIn.Id &&
                    gp.NodeOut.Id == nodeOut.Id);

                // Create the connection
                IGene gene = new Gene(
                    nodeOut, 
                    selectedNodeIn, 
                    existingGene != null ? existingGene.Innovation : GetNextInnovation()
                );

                Genes.Add(gene);

                return gene;
            }

            return null;
        }

        public INode MutateAddNode()
        {
            int innovation = (int)RandomHelpers.GetRandomBiasingLow(
                Genes.Min(g => g.Innovation), Genes.Max(g => g.Innovation)
            );

            IGene oldGene = Genes.FirstOrDefault(g => g.Innovation <= innovation);

            if (oldGene == null)
                oldGene = Genes[new Random().Next(Genes.Count)];

            INode newNode = new Node(NodeType.Hidden);

            // Disable the old connection
            oldGene.IsExpressed = false;

            // Add new connections
            IGene newGeneIn = new Gene(
                oldGene.NodeOut,
                newNode,
                GetNextInnovation(),
                1
            );

            IGene newGeneOut = new Gene(
                newNode,
                oldGene.NodeIn,
                GetNextInnovation(),
                oldGene.Weight
            );

            Nodes.Add(newNode);

            Genes.Add(newGeneIn);
            Genes.Add(newGeneOut);

            return newNode;
        }

        public void MutateGeneWeights(GeneMutationType mutationType, double power, double rate)
        {
            int geneCount = 0;
            double genomeTail = Genes.Count * 0.8;

            bool severe = new Random().NextDouble() > GeneParameters.MutateGeneWeightSevereProbability;
            
            double adjustmentProbability = 0;
            double replacementProbability = 0;

            Genes.ForEach(g => 
            {
                if (severe)
                {
                    adjustmentProbability = GeneParameters.MutateGeneWeightSevereAdjustmentProbability;
                    replacementProbability = GeneParameters.MutateGeneWeightSevereReplacementProbability;
                }
                else if (Genes.Count >= GeneParameters.GeneCountThreshold && geneCount > genomeTail)
                {
                    adjustmentProbability = GeneParameters.MutateGeneWeightAdjustmentProbability;
                    replacementProbability = GeneParameters.MutateGeneWeightReplacementProbability;
                }
                else
                {
                    if (new Random().NextDouble() > 0.5)
                    {
                        adjustmentProbability = 1 - rate;
                        replacementProbability = 1 - rate - 0.1;
                    }
                    else
                    {
                        adjustmentProbability = 1 - rate;
                        replacementProbability = 1 - rate;
                    }
                }

                if (mutationType == GeneMutationType.AdjustWeight)
                {
                    double choice = new Random().NextDouble();

                    if (choice > adjustmentProbability)
                        g.MutateWeight(mutationType, power);
                    else if (choice > replacementProbability)
                        g.MutateWeight(mutationType, power);
                }
                else if (mutationType == GeneMutationType.ReplaceWeight)
                    g.MutateWeight(mutationType, power);

                if (g.Weight > GeneParameters.GeneWeightMaximum)
                    g.Weight = GeneParameters.GeneWeightMaximum;
                else if (g.Weight < GeneParameters.GeneWeightMinimum)
                    g.Weight = GeneParameters.GeneWeightMinimum;

                geneCount++;
            });
        }

        public void MutateGeneReExpress()
        {
            List<IGene> unexpressedGenes = Genes.Where(g => !g.IsExpressed).ToList();

            if (unexpressedGenes.Any())
                unexpressedGenes[new Random().Next(unexpressedGenes.Count)].MutateExpressionEnable();
        }

        public void MutateGeneExpression()
        {
            IGene gene = Genes[new Random().Next(Genes.Count)];

            Genes[new Random().Next(Genes.Count)].MutateExpression();

            // Avoid Node Isolation
            if (!gene.IsExpressed && !Genes.Any(g => g.NodeIn == gene.NodeIn))
                gene.MutateExpressionEnable();
        }

        public List<IGene> GetExpressedGenesByNodeOut(INode nodeOut)
        {
            return Genes.Where(g => g.NodeOut == nodeOut && g.IsExpressed).ToList();
        }

        public bool IsCompatible(IGenome genome)
        {
            // Calculate compatibility distance between the two genomes
            GeneComparer comparer = new GeneComparer();

            IEnumerable<IGene> matchingGenes = genome.Genes.Intersect(Genes, comparer);

            if (matchingGenes.Count() == 0)
                return false;

            double averageWeightDifference = matchingGenes.Average(g =>
                Math.Abs(g.Weight - Genes.First(gs => gs.Innovation == g.Innovation).Weight)
            );

            IEnumerable<IGene> differingGenes = genome.Genes.Concat(Genes).Except(matchingGenes, comparer);

            // Disjoint/Excess genes
            IEnumerable<IGene> disjointGenes = differingGenes
                .Where(dg => dg.Innovation < matchingGenes.Max(g => g.Innovation));

            int disjointCount = disjointGenes.ToList().Count();
            int excessCount = differingGenes.Except(disjointGenes, comparer).ToList().Count();

            IGenome largestGenome = Size() >= genome.Size() ? this : genome;
            int normalisation = largestGenome.Size() > GenomeParameters.GeneNormalisationThreshold ? largestGenome.Size() : 1;

            double compatibilityDistance = ((disjointCount * GenomeParameters.DisjointGeneCompatibilityFactor) / normalisation) +
                ((excessCount * GenomeParameters.ExcessGeneCompatibilityFactor) / normalisation) +
                (averageWeightDifference * GenomeParameters.AverageWeightDifferenceFactor);

            return compatibilityDistance < GenomeParameters.CompatibilityDistanceThreshold;
        }

        public double[] Activate(double[] inputs, Func<double, double> activator = null)
        {
            // Reset any previous activations
            Nodes.ToList().ForEach(n => n.ResetActivation());

            // Activate all of our input nodes
            Nodes.Where(n => n.Type == NodeType.Input).ToList().ForEach(n => n.Activate(inputs[n.ActivationIndex]));

            // Traverse the network and activate each node
            List<INode> activationQueue = Nodes.Where(n => !n.IsActivated).ToList();

            // Set Activator
            activator = activator ?? Activators.ReLU;

            while (activationQueue.Any())
            {
                INode node = activationQueue.First();
                List<IGene> genes = Genes.Where(g => g.NodeIn == node && g.IsExpressed).ToList();

                activationQueue.Remove(node);

                // Remove any genes with unactivated Node Outs
                if (genes.Any(g => !g.NodeOut.IsActivated))
                {
                    genes.RemoveAll(g => !g.NodeOut.IsActivated);

                    // Re-activate this node later
                    activationQueue.Add(node);
                }

                node.Activate(activator(genes.Sum(g => g.Weight * g.NodeOut.Activation)));
            }

            return Nodes.Where(n => n.Type == NodeType.Output)
                .OrderBy(n => n.ActivationIndex)
                .Select(n => n.Activation)
                .ToArray();
        }

        public IGenome Breed(IGenome mate)
        {
            List<IGene> offspringGenes = new List<IGene>();

            // Find disjoint or excess genes
            IEnumerable<IGene> genes = Genes.Concat(mate.Genes);
            IGenome mostFitGenome = Fitness > mate.Fitness ? this : mate;

            foreach (IGene gene in genes)
            {
                if (offspringGenes.Any(g => g.Innovation == gene.Innovation))
                    continue;

                List<IGene> comparedGenes = genes.Where(g => g.Innovation == gene.Innovation).ToList();

                if (comparedGenes.Count > 1) // Matching gene
                {
                    IGene chosenGene = comparedGenes[new Random().Next(2)].Copy();

                    if (comparedGenes[0].IsExpressed == false || comparedGenes[1].IsExpressed == false)
                        chosenGene.IsExpressed = new Random().NextDouble() < GeneParameters.InheritedGeneChanceOfReExpression;

                    offspringGenes.Add(chosenGene);
                }
                else if (comparedGenes.Count == 1) // Disjoint or excess gene
                {
                    bool addGene = (Fitness == mate.Fitness && new Random().Next(2) == 0) ||
                        (mostFitGenome.Genes.FirstOrDefault(mfg => mfg == comparedGenes[0]) != null);

                    if (addGene == true && !offspringGenes.Any(og => og.Innovation == comparedGenes[0].Innovation))
                        offspringGenes.Add(comparedGenes[0].Copy());
                }
            }

            return new Genome()
            {
                Nodes = GenomeHelper.GetNodesFromGenes(offspringGenes).ToList(),
                Genes = offspringGenes
            };
        }

        public static int GetNextInnovation()
        {
            currentInnovation += 1;
            return currentInnovation;
        }

        public static void ResetInnovation()
        {
            currentInnovation = 0;
        }
    }

    public static class GenomeHelper
    {
        public static IEnumerable<INode> GetNodesFromGenes(IEnumerable<IGene> genes)
        {
            return genes.Select(og => og.NodeIn)
                .Concat(genes.Select(og => og.NodeOut))
                .Distinct(new NodeComparer());
        }
    }
}
