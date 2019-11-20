using NEAT.Genotype.Constants;
using NEAT.Interfaces;
using System;
using System.Collections.Generic;

namespace NEAT.Genotype
{
    public class Node : INode
    {
        public Guid Id { get; set; }
        public NodeType Type { get; }
        public int ActivationIndex { get; set; }
        public double Activation { get; set; }
        public bool IsActivated { get; set; }

        public Node(NodeType type, int activationIndex = -1)
        {
            Id = Guid.NewGuid();
            Type = type;
            ActivationIndex = activationIndex;
        }

        public INode Copy()
        {
            return new Node(Type, ActivationIndex) { Id = Id };
        }

        public void Activate(double value)
        {
            IsActivated = true;
            Activation = value;
        }

        public void ResetActivation()
        {
            IsActivated = false;
            Activation = 0;
        }
    }

    public class NodeComparer : IEqualityComparer<INode>
    {
        public bool Equals(INode a, INode b)
        {
            return a.Id == b.Id;
        }

        public int GetHashCode(INode a)
        {
            return a.Id.GetHashCode();
        }
    }
}
