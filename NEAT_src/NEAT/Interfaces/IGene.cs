using NEAT.Genotype.Constants;

namespace NEAT.Interfaces
{
    public interface IGene : ICell
    {
        INode NodeOut { get; set; }
        INode NodeIn { get; set; }
        double Weight { get; set; }
        bool IsExpressed { get; set; }
        int Innovation { get; }

        double MutateWeight(GeneMutationType mutationType, double power);
        void MutateExpression();
        void MutateExpressionEnable();
        IGene Copy();
    }
}
