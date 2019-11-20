using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NEAT.Test
{
    [TestClass]
    public class PopulationTest : Test
    {
        private Population population;

        [TestInitialize]
        public new void Init()
        {
            population = new Population(3, 1);
            population.Spawn(10);
        }

        //[TestMethod]
        //public void SpeciateTest()
        //{
        //    // Create some diversity
        //    Enumerable.Range(0, 2).ToList().ForEach(i =>
        //    {
        //        population.Genomes.ToList().ForEach(g =>
        //        {
        //            int mutateOrNot = new Random().Next(3);

        //            if (mutateOrNot == 0)
        //                g.MutateAddGene();
        //            else if (mutateOrNot == 1)
        //                g.MutateAddNode();

        //            IGenome parent = population.Genomes[new Random().Next(population.Genomes.Count)];
        //            IGenome mate = population.Genomes
        //                .Where(ge => ge != parent)
        //                .ToList()[new Random().Next(population.Genomes.Count - 1)];

        //            double[] inputs = new double[] { 0.3, 0.4, 0.5 };

        //            IPhenome parentPhenome = new Phenome(parent, inputs, (x, y) => 1);
        //            IPhenome matePhenome = new Phenome(mate, inputs, (x, y) => 1);

        //            population.Genomes.Add(parentPhenome.Breed(matePhenome));
        //        });
        //    });
            
        //    population.Speciate();
        //}
    }
}
