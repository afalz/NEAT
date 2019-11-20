using System;
using System.Collections.Generic;
using System.Text;

namespace NEAT.Utilities
{
    public static class Activators
    {
        public static double ReLU(double x)
        {
            return Math.Max(0, x);
        }

        public static double LogSigmoid(double x)
        {
            if (x < -45.0) return 0.0;
            else if (x > 45.0) return 1.0;
            else return 1.0 / (1.0 + Math.Exp(-x));
        }

        public static double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }
    }
}
