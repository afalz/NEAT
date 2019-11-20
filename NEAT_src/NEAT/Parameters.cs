namespace NEAT
{
    public static class GenerationParameters
    {
        public static int ChampionCopyMinimumGenomes = 5;
        public static double InterspeciesBreedingRate = 0.001;
        public static double MutateOnlyProbability = 0.25;
        public static double MutateAddNodeProbability = 0.03;
        public static double MutateAddGeneProbability = 0.05;
        public static double MutateGeneWeightProbability = 0.2;
        public static double MutateAllGeneWeightsProbability = 0.8;
        public static double MutateGeneExpressionProbability = 0.2;
        public static double MutateOffspringProbability = 0.02;
        public static double MutateGeneReExpressionProbability = 0.75;
    }

    public static class GenomeParameters
    {
        public static double DisjointGeneCompatibilityFactor = 1;
        public static double ExcessGeneCompatibilityFactor = 1;
        public static double AverageWeightDifferenceFactor = 0.4;
        public static double CompatibilityDistanceThreshold = 3;
        public static int GeneNormalisationThreshold = 20;
        public static bool AllowOutputOutGenes = false;
    }

    public static class GeneParameters
    {
        public static double MutateGeneWeightFactor = 1.0;
        public static double MutateExpressionProbability = 0.2;
        public static double InheritedGeneChanceOfReExpression = 0.25;
        public static double GeneWeightMinimum = -8;
        public static double GeneWeightMaximum = 8;
        public static double MutateGeneWeightSevereAdjustmentProbability = 0.3;
        public static double MutateGeneWeightSevereReplacementProbability = 0.1;
        public static double MutateGeneWeightAdjustmentProbability = 0.5;
        public static double MutateGeneWeightReplacementProbability = 0.3;
        public static double MutateGeneWeightSevereProbability = 0.5;
        public static double GeneCountThreshold = 10;
    }

    public static class SpeciesParameters
    {
        public static double NaturalSelectionFactor = 0.2;
        public static int DropOffAge = 15;
        public static double DropOffAgePenaltyFactor = 0.01;
        public static double AgeSignificance = 1;
    }
}