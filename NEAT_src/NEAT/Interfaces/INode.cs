using NEAT.Genotype.Constants;
using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Interfaces
{
    public interface INode : ICell
    {
        Guid Id { get; }
        NodeType Type { get; }
        double Activation { get; set; }
        int ActivationIndex { get; set; }
        bool IsActivated { get; set; }

        INode Copy();
        void Activate(double value);
        void ResetActivation();
    }
}