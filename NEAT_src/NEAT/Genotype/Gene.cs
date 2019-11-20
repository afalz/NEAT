using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using System;
using System.Collections.Generic;

namespace NEAT.Genotype
{
    public class Gene : IGene
    {
        public INode NodeOut { get; set; }
        public INode NodeIn { get; set; }
        public double Weight { get; set; }
        public bool IsExpressed { get; set; }
        public int Innovation { get; }

        public Gene(INode nodeOut, INode nodeIn, int innovation, double? weight = null)
        {
            NodeOut = nodeOut;
            NodeIn = nodeIn;
            Innovation = innovation;
            IsExpressed = true;

            if (!weight.HasValue)
                Weight = GetWeight();
            else
                Weight = weight.Value;
        }

        public double MutateWeight(GeneMutationType mutationType, double power)
        {
            double factor = (new Random().NextDouble() < 0.5 ? -1 : 1);
            double weight = new Random().NextDouble() * power * factor;

            if (mutationType == GeneMutationType.AdjustWeight)
                Weight += weight;
            else if (mutationType == GeneMutationType.ReplaceWeight)
                Weight = weight;

            return weight;
        }

        public void MutateExpression()
        {
            IsExpressed = new Random().NextDouble() > GeneParameters.MutateExpressionProbability ? IsExpressed : !IsExpressed;
        }

        public void MutateExpressionEnable()
        {
            IsExpressed = true;
        }

        public IGene Copy()
        {
            return new Gene(NodeOut, NodeIn, Innovation, Weight);
        }

        private double GetWeight()
        {
            return Weight = new Random().NextDouble();
        }
    }

    public class GeneComparer : IEqualityComparer<IGene>
    {
        public bool Equals(IGene a, IGene b)
        {
            return a.Innovation == b.Innovation;
        }

        public int GetHashCode(IGene a)
        {
            return a.Innovation.GetHashCode();
        }
    }
}
