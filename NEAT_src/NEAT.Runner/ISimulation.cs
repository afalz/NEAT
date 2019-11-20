using NEAT.Interfaces;

namespace NEAT.Runner
{
    public interface ISimulation
    {
        IGenome Simulate();
    }
}
