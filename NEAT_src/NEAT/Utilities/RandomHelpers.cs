using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Utilities
{
    public static class RandomHelpers
    {
        public static double GetRandomBiasingLow(int min, int max)
        {
            return Math.Floor(Math.Abs(new Random().NextDouble() - new Random().NextDouble()) * (1 + max - min) + min);
        }
    }
}
