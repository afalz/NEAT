using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using System;

namespace NEAT.Utilities
{
    public static class DotGraph
    {
        public static string Get(IGenome genome)
        {
            string dotCode = "digraph G { ";

            genome.Nodes.ForEach(n =>
            {
                dotCode += String.Format("\"{0}\" [ color = \"{1}\" ] ",
                    n.Id.ToString().Substring(32) + " " + Math.Round(n.Activation, 2),
                    n.Type == NodeType.Input ? "blue" : (n.Type == NodeType.Output ? "red" : "orange")
                );
            });

            genome.Genes.ForEach(g =>
            {
                dotCode += String.Format("\"{0}\" -> \"{1}\" [ label = \"{2}\", color = \"{3}\" ] ", 
                    g.NodeOut.Id.ToString().Substring(32) + " " + Math.Round(g.NodeOut.Activation, 2),
                    g.NodeIn.Id.ToString().Substring(32) + " " + Math.Round(g.NodeIn.Activation, 2),
                    Math.Round(g.Weight, 2),
                    g.IsExpressed ? "black" : "lightgrey"
                );
            });

            dotCode += "}";

            return dotCode;
        }
    }
}
