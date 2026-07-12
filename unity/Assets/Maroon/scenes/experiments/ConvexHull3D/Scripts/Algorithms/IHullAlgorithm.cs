namespace Maroon.ComputerScience.ConvexHull3D
{

    public interface IHullAlgorithm
    {
        string Name
        {
            get; 
        }
        
        string[] PseudocodeLines
        { 
            get;
        }

        void Run(ConvexHull3D context);
    }
}